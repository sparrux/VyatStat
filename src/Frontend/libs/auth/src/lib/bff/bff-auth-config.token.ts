import { InjectionToken, Provider } from '@angular/core';

export interface BffAuthConfig {
  /** Base URL of the BFF (e.g. Hub.Web). */
  bffBaseUrl: string;
  /**
   * Absolute URL passed as `returnUrl` to `/auth/login`.
   * Defaults to `window.location.href` at login time.
   */
  loginReturnUrl?: string;
}

export const BFF_AUTH_CONFIG = new InjectionToken<BffAuthConfig>('BFF_AUTH_CONFIG');

export function provideBffAuthConfig(config: BffAuthConfig): Provider {
  return { provide: BFF_AUTH_CONFIG, useValue: config };
}
