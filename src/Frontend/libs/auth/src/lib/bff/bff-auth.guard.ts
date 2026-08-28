import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { map } from 'rxjs';
import { BffAuthService } from './bff-auth.service';

export const bffAuthGuard: CanActivateFn = () => {
  const auth = inject(BffAuthService);

  if (auth.isAuthenticated()) {
    return true;
  }

  if (auth.sessionResolved()) {
    auth.login();
    return false;
  }

  return auth.checkSession().pipe(
    map((user) => {
      if (user) {
        return true;
      }
      auth.login();
      return false;
    }),
  );
};
