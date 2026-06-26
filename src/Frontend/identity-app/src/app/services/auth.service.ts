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
import { environment } from '../../environments/environment';
import {
  OAuthTokenResponse,
  UpdatePasswordRequest,
  UserClaims,
  UserProfile,
} from '../models/auth.model';
import { getJwtExpirationUtcMs } from '../utils/jwt-exp';

/** Refresh this many ms before JWT exp (target ~30–60 s window with skew). */
const PROACTIVE_LEAD_MS = 50_000;
/** Extra safety for client clock drift and slow networks. */
const CLOCK_SKEW_MS = 10_000;

const OAUTH_SCOPE = 'openid profile offline_access';
const OAUTH_STATE_KEY = 'oauth_state';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly authServerUrl = environment.authServerUrl;
  private readonly clientId = environment.clientId;
  private readonly apiAudience = environment.apiAudience;
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

  isValidAuthorizeReturnUrl(returnUrl: string): boolean {
    try {
      const url = new URL(returnUrl);
      const authority = new URL(this.authServerUrl);
      if (url.protocol !== authority.protocol || url.host !== authority.host) {
        return false;
      }
      const path = url.pathname.replace(/\/$/, '');
      return path.toLowerCase().endsWith('/connect/authorize');
    } catch {
      return false;
    }
  }

  async hasIdpCookieSession(): Promise<boolean> {
    try {
      const response = await fetch(`${this.authServerUrl}/account/session`, {
        method: 'GET',
        credentials: 'include',
      });
      return response.ok;
    } catch {
      return false;
    }
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

  async logout(): Promise<void> {
    this.clearLocalSession();

    try {
      await fetch(`${this.authServerUrl}/account/logout`, {
        method: 'POST',
        credentials: 'include',
      });
    } catch {
      // IdP session cleanup is best-effort; local tokens are already cleared.
    }
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

  register(username: string, password: string): Observable<unknown> {
    const url = `${this.authServerUrl}/register`;
    const body = { login: username, password };
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post(url, body, { headers });
  }

  /**
   * Starts the OAuth authorization code flow (GET /connect/authorize).
   * Uses the IdP cookie when present so the user can skip the login form.
   */
  async startAuthorizationFlow(): Promise<void> {
    window.location.href = await this.buildAuthorizeReturnUrl();
  }

  /**
   * Signs in via IdP cookie session, then continues the OAuth authorize redirect.
   */
  async login(username: string, password: string, authorizeReturnUrl?: string | null): Promise<void> {
    const response = await fetch(`${this.authServerUrl}/account/login`, {
      method: 'POST',
      credentials: 'include',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ login: username, password }),
    });

    if (!response.ok) {
      throw new Error('Login failed');
    }

    window.location.href = authorizeReturnUrl ?? (await this.buildAuthorizeReturnUrl());
  }

  /**
   * Builds the GET /connect/authorize URL and stores a fresh PKCE verifier and state.
   */
  async buildAuthorizeReturnUrl(): Promise<string> {
    const verifier = this.generateVerifier();
    const state = this.generateState();
    localStorage.setItem('code_verifier', verifier);
    sessionStorage.setItem(OAUTH_STATE_KEY, state);
    const challenge = await this.generateChallenge(verifier);

    const params = new URLSearchParams({
      client_id: this.clientId,
      response_type: 'code',
      scope: OAUTH_SCOPE,
      redirect_uri: this.redirectUri,
      state,
      code_challenge: challenge,
      code_challenge_method: 'S256',
    });

    return `${this.authServerUrl}/connect/authorize?${params.toString()}`;
  }

  /**
   * Validates the OAuth state from the callback query and consumes it (one-time use).
   */
  validateAndConsumeOAuthState(receivedState: string | null | undefined): boolean {
    const expected = sessionStorage.getItem(OAUTH_STATE_KEY);
    sessionStorage.removeItem(OAUTH_STATE_KEY);

    if (!expected || !receivedState || expected !== receivedState) {
      this.clearOAuthTransientState();
      return false;
    }

    return true;
  }

  clearOAuthTransientState(): void {
    sessionStorage.removeItem(OAUTH_STATE_KEY);
    localStorage.removeItem('code_verifier');
  }

  exchangeCodeForToken(code: string): Observable<OAuthTokenResponse> {
    const verifier = localStorage.getItem('code_verifier') ?? '';

    const body = new HttpParams()
      .set('client_id', this.clientId)
      .set('aud', this.apiAudience)
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
  ensureAccessTokenIfNeeded(): Observable<void> {
    if (localStorage.getItem('access_token')) {
      return of(undefined);
    }
    if (!localStorage.getItem('refresh_token')) {
      return throwError(() => new Error('Missing refresh token'));
    }
    return this.refreshAccessTokenSilently();
  }

  private generateState(): string {
    const array = new Uint8Array(32);
    crypto.getRandomValues(array);
    return Array.from(array, (byte) => byte.toString(16).padStart(2, '0')).join('');
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

  private runRefreshRequest(refreshToken: string): Promise<OAuthTokenResponse> {
    const body = new HttpParams()
      .set('client_id', this.clientId)
      .set('aud', this.apiAudience)
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

  private clearLocalSession(): void {
    this.authEpoch++;
    this.clearProactiveRefreshTimer();
    this.clearOAuthTransientState();
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
  }

  /**
   * Clears stored tokens and restarts OAuth (uses IdP cookie when still valid).
   */
  invalidateSessionAndRedirectToLogin(): void {
    this.clearLocalSession();
    void this.startAuthorizationFlow();
  }
}
