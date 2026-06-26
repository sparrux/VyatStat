import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth';

const AUTH_RETRY_HEADER = 'X-Auth-Retry';

function isTrackerApiRequest(url: string, trackerApiUrl: string): boolean {
  return url.startsWith(trackerApiUrl);
}

function isAuthServerOAuthRequest(url: string, authBaseUrl: string): boolean {
  return (
    url.startsWith(authBaseUrl)
    && (url.includes('/connect/token') || url.includes('/connect/authorize'))
  );
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const trackerBase = auth.getTrackerApiUrl();
  const authBase = auth.getAuthServerUrl();

  if (isAuthServerOAuthRequest(req.url, authBase) || !isTrackerApiRequest(req.url, trackerBase)) {
    return next(req);
  }

  const token = localStorage.getItem('access_token');
  const authedReq =
    token && !req.headers.has('Authorization')
      ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
      : req;

  return next(authedReq).pipe(
    catchError((err: unknown) => {
      if (!(err instanceof HttpErrorResponse) || err.status !== 401) {
        return throwError(() => err);
      }
      if (req.headers.has(AUTH_RETRY_HEADER)) {
        auth.invalidateSessionAndRestartAuth();
        return throwError(() => err);
      }
      return auth.refreshAccessTokenSilently().pipe(
        switchMap(() => {
          const refreshedToken = localStorage.getItem('access_token');
          if (!refreshedToken) {
            auth.invalidateSessionAndRestartAuth();
            return throwError(() => err);
          }
          const retried = req.clone({
            setHeaders: {
              Authorization: `Bearer ${refreshedToken}`,
              [AUTH_RETRY_HEADER]: '1',
            },
          });
          return next(retried);
        }),
        catchError((refreshErr) => {
          auth.invalidateSessionAndRestartAuth();
          return throwError(() => refreshErr);
        }),
      );
    }),
  );
};
