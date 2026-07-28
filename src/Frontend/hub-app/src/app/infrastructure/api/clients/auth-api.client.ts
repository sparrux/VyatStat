import { Injectable, computed, inject } from '@angular/core';
import { BffAuthService } from '@vyatka-tracker/auth';
import { Observable, map } from 'rxjs';

import { IAuthApi } from '../../../application/contracts/auth-api.contract';
import { User } from '../../../application/models/user.model';
import { mapBffSessionUserToUser } from '../mappers/user.mapper';

/**
 * Infrastructure adapter over shared BFF auth.
 * Owns HTTP/session details; exposes domain User to Application.
 */
@Injectable()
export class AuthApiClient implements IAuthApi {
  private readonly bffAuth = inject(BffAuthService);

  readonly user = computed(() => {
    const sessionUser = this.bffAuth.user();
    return sessionUser ? mapBffSessionUserToUser(sessionUser) : null;
  });

  readonly sessionResolved = this.bffAuth.sessionResolved;
  readonly isAuthenticated = this.bffAuth.isAuthenticated;

  onAppBootstrap(): Promise<void> {
    return this.bffAuth.onAppBootstrap();
  }

  checkSession(): Observable<User | null> {
    return this.bffAuth.checkSession().pipe(
      map((sessionUser) =>
        sessionUser ? mapBffSessionUserToUser(sessionUser) : null,
      ),
    );
  }

  login(returnUrl?: string): void {
    this.bffAuth.login(returnUrl);
  }

  logout(): Observable<void> {
    return this.bffAuth.logout();
  }
}
