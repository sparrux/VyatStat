import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { AUTH_API } from '../contracts/auth-api.contract';
import { User } from '../models/user.model';

/**
 * Application use-cases for authentication and session.
 * Presentation must depend on this service, not on Infrastructure.
 */
@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  private readonly authApi = inject(AUTH_API);

  readonly user = this.authApi.user;
  readonly sessionResolved = this.authApi.sessionResolved;
  readonly isAuthenticated = this.authApi.isAuthenticated;

  onAppBootstrap(): Promise<void> {
    return this.authApi.onAppBootstrap();
  }

  checkSession(): Observable<User | null> {
    return this.authApi.checkSession();
  }

  login(returnUrl?: string): void {
    this.authApi.login(returnUrl);
  }

  logout(): Observable<void> {
    return this.authApi.logout();
  }
}
