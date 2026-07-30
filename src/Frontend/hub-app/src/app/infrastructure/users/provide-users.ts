import { EnvironmentProviders, Provider, makeEnvironmentProviders } from '@angular/core';

import { USER_API } from '../../application/contracts/user-api.contract';
import { UserApiClient } from '../api/clients/user-api.client';

/**
 * Wires Users API infrastructure and binds IUserApi → UserApiClient.
 */
export function provideHubUsersInfrastructure(): EnvironmentProviders {
  const providers: Provider[] = [
    UserApiClient,
    {
      provide: USER_API,
      useExisting: UserApiClient,
    },
  ];

  return makeEnvironmentProviders(providers);
}
