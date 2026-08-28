import { EnvironmentProviders, Provider, makeEnvironmentProviders } from '@angular/core';

import { GROUP_API } from '../../application/contracts/group-api.contract';
import { GroupApiClient } from '../api/clients/group-api.client';

/**
 * Wires Groups API infrastructure and binds IGroupApi → GroupApiClient.
 */
export function provideHubGroupsInfrastructure(): EnvironmentProviders {
  const providers: Provider[] = [
    GroupApiClient,
    {
      provide: GROUP_API,
      useExisting: GroupApiClient,
    },
  ];

  return makeEnvironmentProviders(providers);
}
