import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { BffAuthService } from './bff-auth.service';

function isBffRequest(url: string, bffBaseUrl: string): boolean {
  return !!bffBaseUrl && url.startsWith(bffBaseUrl);
}

/** Sends cookies to the BFF and restarts login on 401 for BFF-scoped requests. */
export const bffCredentialsInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(BffAuthService);
  const bffBase = auth.getBffBaseUrl();

  if (!isBffRequest(req.url, bffBase)) {
    return next(req);
  }

  const credentialsReq = req.clone({ withCredentials: true });

  return next(credentialsReq).pipe(
    catchError((err: unknown) => {
      if (err instanceof HttpErrorResponse && err.status === 401) {
        // Avoid login loop on the session probe itself.
        if (!req.url.includes('/auth/session') && !req.url.includes('/auth/logout')) {
          auth.login();
        }
      }
      return throwError(() => err);
    }),
  );
};
