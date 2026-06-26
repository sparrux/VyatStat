import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const readUsersGuard: CanActivateFn = async () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (!auth.isAuthenticated()) {
    void auth.startAuthorizationFlow();
    return false;
  }

  try {
    const profile = await firstValueFrom(auth.getProfile());
    const claims = await firstValueFrom(auth.getUserPermissions(profile.id));

    if (claims.readUsers) {
      return true;
    }

    return router.createUrlTree(['/account']);
  } catch {
    void auth.startAuthorizationFlow();
    return false;
  }
};
