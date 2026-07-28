import { InjectionToken, Signal } from '@angular/core';
import { Observable } from 'rxjs';

import { User } from '../models/user.model';

/**
 * Port for authentication / session access.
 * Implemented by Infrastructure; consumed by Application services.
 */
export interface IAuthApi {
  readonly user: Signal<User | null>;
  readonly sessionResolved: Signal<boolean>;
  readonly isAuthenticated: Signal<boolean>;

  onAppBootstrap(): Promise<void>;
  checkSession(): Observable<User | null>;
  login(returnUrl?: string): void;
  logout(): Observable<void>;
}

export const AUTH_API = new InjectionToken<IAuthApi>('IAuthApi');
