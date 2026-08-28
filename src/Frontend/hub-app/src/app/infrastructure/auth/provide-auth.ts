import { EnvironmentProviders, Provider, makeEnvironmentProviders } from '@angular/core';
import {
  bffCredentialsInterceptor,
  provideBffAuthConfig,
} from '@vyatka-tracker/auth';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { AUTH_API } from '../../application/contracts/auth-api.contract';
import { AuthApiClient } from '../api/clients/auth-api.client';

export interface HubAuthInfrastructureOptions {
  bffBaseUrl: string;
}

/**
 * Wires BFF auth infrastructure and binds IAuthApi → AuthApiClient.
 */
export function provideHubAuthInfrastructure(
  options: HubAuthInfrastructureOptions,
): EnvironmentProviders {
  const providers: Provider[] = [
    provideBffAuthConfig({
      bffBaseUrl: options.bffBaseUrl,
    }),
    AuthApiClient,
    {
      provide: AUTH_API,
      useExisting: AuthApiClient,
    },
  ];

  return makeEnvironmentProviders([
    ...providers,
    provideHttpClient(withInterceptors([bffCredentialsInterceptor])),
  ]);
}
