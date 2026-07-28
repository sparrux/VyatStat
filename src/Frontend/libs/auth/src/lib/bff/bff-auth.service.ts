import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, catchError, firstValueFrom, map, of, tap, throwError } from 'rxjs';
import { BFF_AUTH_CONFIG } from './bff-auth-config.token';
import { BffSessionUser } from './bff-session.model';

@Injectable({
  providedIn: 'root',
})
export class BffAuthService {
  private readonly config = inject(BFF_AUTH_CONFIG);
  private readonly http = inject(HttpClient);

  private readonly userSignal = signal<BffSessionUser | null>(null);
  private readonly sessionResolvedSignal = signal(false);

  readonly user = this.userSignal.asReadonly();
  readonly sessionResolved = this.sessionResolvedSignal.asReadonly();
  readonly isAuthenticated = computed(() => this.userSignal() !== null);

  getBffBaseUrl(): string {
    return this.config.bffBaseUrl.replace(/\/$/, '');
  }

  onAppBootstrap(): Promise<void> {
    return firstValueFrom(this.checkSession()).then(() => undefined);
  }

  checkSession(): Observable<BffSessionUser | null> {
    const url = `${this.getBffBaseUrl()}/auth/session`;
    return this.http
      .get<BffSessionUser>(url, { withCredentials: true })
      .pipe(
        tap((user) => {
          this.userSignal.set(user);
          this.sessionResolvedSignal.set(true);
        }),
        catchError((err: unknown) => {
          if (err instanceof HttpErrorResponse && err.status === 401) {
            this.clearSession();
            return of(null);
          }
          this.clearSession();
          return of(null);
        }),
        map((user) => user),
      );
  }

  login(returnUrl?: string): void {
    const resolvedReturnUrl =
      returnUrl
      ?? this.config.loginReturnUrl
      ?? window.location.href;
    const loginUrl = new URL(`${this.getBffBaseUrl()}/auth/login`);
    loginUrl.searchParams.set('returnUrl', resolvedReturnUrl);
    window.location.assign(loginUrl.toString());
  }

  logout(): Observable<void> {
    const url = `${this.getBffBaseUrl()}/auth/logout`;
    return this.http.post<void>(url, null, { withCredentials: true }).pipe(
      tap(() => this.clearSession()),
      catchError((err: unknown) => {
        this.clearSession();
        return throwError(() => err);
      }),
      map(() => undefined),
    );
  }

  private clearSession(): void {
    this.userSignal.set(null);
    this.sessionResolvedSignal.set(true);
  }
}
