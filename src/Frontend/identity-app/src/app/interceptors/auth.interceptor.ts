import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth';

const AUTH_RETRY_HEADER = 'X-Auth-Retry';

function shouldHandleUnauthorized(url: string, authBaseUrl: string): boolean {
  if (!url.startsWith(authBaseUrl)) {
    return false;
  }
  if (url.includes('/connect/token') || url.includes('/connect/authorize')) {
    return false;
  }
  const pathOrQuery = url.slice(authBaseUrl.length);
  if (pathOrQuery === '/register' || pathOrQuery.startsWith('/register?')) {
    return false;
  }
  return true;
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const base = auth.getAuthServerUrl();

  if (!shouldHandleUnauthorized(req.url, base)) {
    return next(req);
  }

  return next(req).pipe(
    catchError((err: unknown) => {
      if (!(err instanceof HttpErrorResponse) || err.status !== 401) {
        return throwError(() => err);
      }
      if (req.headers.has(AUTH_RETRY_HEADER)) {
        return throwError(() => err);
      }
      return auth.refreshAccessTokenSilently().pipe(
        switchMap(() => {
          const token = localStorage.getItem('access_token');
          if (!token) {
            auth.invalidateSessionAndRedirectToLogin();
            return throwError(() => err);
          }
          const retried = req.clone({
            setHeaders: {
              Authorization: `Bearer ${token}`,
              [AUTH_RETRY_HEADER]: '1',
            },
          });
          return next(retried);
        }),
        catchError((refreshErr) => {
          auth.invalidateSessionAndRedirectToLogin();
          return throwError(() => refreshErr);
        }),
      );
    }),
  );
};
