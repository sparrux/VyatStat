import { HttpBackend, HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import {
  Observable,
  catchError,
  defer,
  firstValueFrom,
  from,
  of,
  switchMap,
  throwError,
} from 'rxjs';
import { getJwtExpirationUtcMs } from '../utils/jwt-exp';

export interface UserProfile {
  id: string;
  userName: string | null;
  email: string | null;
  claims: UserClaims | null;
}

export interface UserClaims {
  isAdmin: boolean;
  readUsers: boolean;
  updateUserPermissions: boolean;
  lockOutUsers: boolean;
}

export interface OAuthTokenResponse {
  access_token: string;
  refresh_token?: string;
  expires_in?: number;
}

export interface UpdatePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

/** Refresh this many ms before JWT exp (target ~30–60 s window with skew). */
const PROACTIVE_LEAD_MS = 50_000;
/** Extra safety for client clock drift and slow networks. */
const CLOCK_SKEW_MS = 10_000;

const OAUTH_SCOPE = 'openid profile offline_access';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly authServerUrl = 'https://localhost:7019';
  private readonly clientId = 'angular-client';
  private readonly redirectUri = window.location.origin + '/callback';

  private readonly http = inject(HttpClient);
  private readonly directHttp = new HttpClient(inject(HttpBackend));
  private readonly router = inject(Router);

  private proactiveRefreshTimer: ReturnType<typeof setTimeout> | null = null;
  private refreshInFlight: Promise<void> | null = null;
  private visibilityHandlerBound = false;
  /** Bumps on logout / invalidation so in-flight refresh cannot resurrect a session. */
  private authEpoch = 0;
  private readonly onDocumentVisibilityChange = (): void => {
    if (typeof document === 'undefined' || document.visibilityState !== 'visible') {
      return;
    }
    const access = localStorage.getItem('access_token');
    const refresh = localStorage.getItem('refresh_token');
    if (!refresh) {
      return;
    }
    if (!access) {
      void this.refreshAccessTokenSilently()
        .pipe(
          catchError(() => {
            this.invalidateSessionAndRedirectToLogin();
            return throwError(() => new Error('Silent refresh failed'));
          }),
        )
        .subscribe();
      return;
    }
    const expMs = getJwtExpirationUtcMs(access);
    if (expMs === null) {
      this.scheduleProactiveRefresh();
      return;
    }
    const threshold = expMs - PROACTIVE_LEAD_MS - CLOCK_SKEW_MS;
    if (Date.now() >= threshold) {
      void this.refreshAccessTokenSilently()
        .pipe(
          catchError(() => {
            this.invalidateSessionAndRedirectToLogin();
            return throwError(() => new Error('Silent refresh failed'));
          }),
        )
        .subscribe();
    } else {
      this.scheduleProactiveRefresh();
    }
  };

  getAuthServerUrl(): string {
    return this.authServerUrl;
  }

  /**
   * Call once at app startup: schedules proactive refresh and tab visibility handling.
   */
  onAppBootstrap(): void {
    this.scheduleProactiveRefresh();
    this.attachVisibilityListener();
  }

  getProfile(): Observable<UserProfile> {
    return defer(() => this.ensureAccessTokenIfNeeded()).pipe(
      switchMap(() => {
        const token = localStorage.getItem('access_token');
        if (!token) {
          return throwError(() => new Error('Missing access token after refresh'));
        }
        const headers = new HttpHeaders({
          Authorization: `Bearer ${token}`,
        });
        return this.http.get<UserProfile>(`${this.authServerUrl}/me`, { headers });
      }),
    );
  }

  updatePassword(request: UpdatePasswordRequest): Observable<void> {
    return defer(() => this.ensureAccessTokenIfNeeded()).pipe(
      switchMap(() => {
        const token = localStorage.getItem('access_token');
        if (!token) {
          return throwError(() => new Error('Missing access token after refresh'));
        }
        const headers = new HttpHeaders({
          Authorization: `Bearer ${token}`,
        });
        return this.http.put<void>(`${this.authServerUrl}/me/password`, request, { headers });
      }),
    );
  }

  getUserPermissions(userId: string): Observable<UserClaims> {
    return defer(() => this.ensureAccessTokenIfNeeded()).pipe(
      switchMap(() => {
        const token = localStorage.getItem('access_token');
        if (!token) {
          return throwError(() => new Error('Missing access token after refresh'));
        }
        const headers = new HttpHeaders({
          Authorization: `Bearer ${token}`,
        });
        return this.http.get<UserClaims>(`${this.authServerUrl}/users/${userId}/permissions`, {
          headers,
        });
      }),
    );
  }

  logout(): void {
    this.authEpoch++;
    this.clearProactiveRefreshTimer();
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('code_verifier');
  }

  /**
   * True if there is a usable access token or a refresh token that can mint a new one.
   */
  isAuthenticated(): boolean {
    return !!localStorage.getItem('access_token') || !!localStorage.getItem('refresh_token');
  }

  /**
   * Persists tokens from the authorization server and (re)schedules background refresh.
   */
  applyOAuthTokens(tokens: OAuthTokenResponse): void {
    localStorage.setItem('access_token', tokens.access_token);
    if (tokens.refresh_token) {
      localStorage.setItem('refresh_token', tokens.refresh_token);
    }
    this.scheduleProactiveRefresh();
  }

  /**
   * Refresh using the refresh_token grant. Uses HttpBackend so interceptors are not involved.
   * Concurrent callers share one in-flight refresh (promise mutex).
   */
  refreshAccessTokenSilently(): Observable<void> {
    const refreshToken = localStorage.getItem('refresh_token');
    if (!refreshToken) {
      return throwError(() => new Error('Missing refresh token'));
    }

    if (!this.refreshInFlight) {
      const epoch = this.authEpoch;
      this.refreshInFlight = this.runRefreshRequest(refreshToken)
        .then((tokens) => {
          if (epoch !== this.authEpoch) {
            return;
          }
          this.applyOAuthTokens(tokens);
        })
        .finally(() => {
          this.refreshInFlight = null;
        });
    }

    return from(this.refreshInFlight);
  }

  private generateVerifier(): string {
    const array = new Uint32Array(56);
    crypto.getRandomValues(array);
    return Array.from(array, (dec) => ('0' + dec.toString(16)).slice(-2)).join('');
  }

  private async generateChallenge(verifier: string): Promise<string> {
    const encoder = new TextEncoder();
    const data = encoder.encode(verifier);
    const hash = await crypto.subtle.digest('SHA-256', data);
    return btoa(String.fromCharCode(...new Uint8Array(hash)))
      .replace(/\+/g, '-')
      .replace(/\//g, '_')
      .replace(/=+$/, '');
  }

  register(username: string, password: string): Observable<unknown> {
    const url = `${this.authServerUrl}/register`;
    const body = { login: username, password };
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post(url, body, { headers });
  }

  async login(username: string, password: string): Promise<void> {
    const verifier = this.generateVerifier();
    localStorage.setItem('code_verifier', verifier);
    const challenge = await this.generateChallenge(verifier);

    const params: Record<string, string> = {
      client_id: this.clientId,
      response_type: 'code',
      scope: OAUTH_SCOPE,
      redirect_uri: this.redirectUri,
      code_challenge: challenge,
      code_challenge_method: 'S256',
      username,
      password,
    };

    const form = document.createElement('form');
    form.method = 'POST';
    form.action = `${this.authServerUrl}/connect/authorize`;

    for (const key of Object.keys(params)) {
      const hiddenField = document.createElement('input');
      hiddenField.type = 'hidden';
      hiddenField.name = key;
      hiddenField.value = params[key]!;
      form.appendChild(hiddenField);
    }

    document.body.appendChild(form);
    form.submit();
  }

  exchangeCodeForToken(code: string): Observable<OAuthTokenResponse> {
    const verifier = localStorage.getItem('code_verifier') ?? '';

    const body = new HttpParams()
      .set('client_id', this.clientId)
      .set('aud', 'vyatka-identity-api')
      .set('grant_type', 'authorization_code')
      .set('code', code)
      .set('redirect_uri', this.redirectUri)
      .set('code_verifier', verifier);

    const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });

    return this.directHttp.post<OAuthTokenResponse>(`${this.authServerUrl}/connect/token`, body.toString(), {
      headers,
    });
  }

  /**
   * Ensures an access token exists when a refresh token is present (OAuth refresh grant).
   */
  private ensureAccessTokenIfNeeded(): Observable<void> {
    if (localStorage.getItem('access_token')) {
      return of(undefined);
    }
    if (!localStorage.getItem('refresh_token')) {
      return throwError(() => new Error('Missing refresh token'));
    }
    return this.refreshAccessTokenSilently();
  }

  private runRefreshRequest(refreshToken: string): Promise<OAuthTokenResponse> {
    const body = new HttpParams()
      .set('client_id', this.clientId)
      .set('aud', 'vyatka-identity-api')
      .set('grant_type', 'refresh_token')
      .set('refresh_token', refreshToken)
      .set('scope', OAUTH_SCOPE);

    const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });

    return firstValueFrom(
      this.directHttp.post<OAuthTokenResponse>(`${this.authServerUrl}/connect/token`, body.toString(), {
        headers,
      }),
    );
  }

  private scheduleProactiveRefresh(): void {
    this.clearProactiveRefreshTimer();

    const access = localStorage.getItem('access_token');
    const refresh = localStorage.getItem('refresh_token');
    if (!refresh) {
      return;
    }
    if (!access) {
      void this.refreshAccessTokenSilently()
        .pipe(
          catchError(() => {
            this.invalidateSessionAndRedirectToLogin();
            return throwError(() => new Error('Restore access token failed'));
          }),
        )
        .subscribe();
      return;
    }

    const expMs = getJwtExpirationUtcMs(access);
    if (expMs === null) {
      return;
    }

    const fireAt = expMs - PROACTIVE_LEAD_MS - CLOCK_SKEW_MS;
    const delayMs = Math.max(0, fireAt - Date.now());

    this.proactiveRefreshTimer = setTimeout(() => {
      this.refreshAccessTokenSilently()
        .pipe(
          catchError(() => {
            this.invalidateSessionAndRedirectToLogin();
            return throwError(() => new Error('Proactive refresh failed'));
          }),
        )
        .subscribe();
    }, delayMs);
  }

  private clearProactiveRefreshTimer(): void {
    if (this.proactiveRefreshTimer !== null) {
      clearTimeout(this.proactiveRefreshTimer);
      this.proactiveRefreshTimer = null;
    }
  }

  private attachVisibilityListener(): void {
    if (this.visibilityHandlerBound || typeof document === 'undefined') {
      return;
    }
    document.addEventListener('visibilitychange', this.onDocumentVisibilityChange);
    this.visibilityHandlerBound = true;
  }

  /**
   * Clears stored tokens and sends the user to login (refresh failure, revoked session).
   */
  invalidateSessionAndRedirectToLogin(): void {
    this.authEpoch++;
    this.clearProactiveRefreshTimer();
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('code_verifier');
    void this.router.navigate(['/login']);
  }
}

