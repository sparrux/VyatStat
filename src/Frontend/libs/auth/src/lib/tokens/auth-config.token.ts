import { InjectionToken, Provider } from '@angular/core';

export interface AuthConfig {
  authServerUrl: string;
  clientId: string;
  apiAudience: string;
  redirectUri?: string;
  /** When set, browser navigates here after local logout completes. */
  postLogoutRedirectUrl?: string;
  /** Base URL for protected resource APIs (e.g. tracker API). */
  resourceApiUrl?: string;
}

export const AUTH_CONFIG = new InjectionToken<AuthConfig>('AUTH_CONFIG');

export function provideAuthConfig(config: AuthConfig): Provider {
  return { provide: AUTH_CONFIG, useValue: config };
}
