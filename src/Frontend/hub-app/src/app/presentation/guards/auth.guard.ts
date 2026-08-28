import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { map } from 'rxjs';

import { AuthenticationService } from '../../application/services/authentication.service';

/**
 * Presentation guard — depends on Application AuthenticationService only.
 */
export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthenticationService);

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
