import { EnvironmentProviders, Provider, makeEnvironmentProviders } from '@angular/core';

import { EVENT_API } from '../../application/contracts/event-api.contract';
import { EventApiClient } from '../api/clients/event-api.client';

/**
 * Wires Events API infrastructure and binds IEventApi → EventApiClient.
 */
export function provideHubEventsInfrastructure(): EnvironmentProviders {
  const providers: Provider[] = [
    EventApiClient,
    {
      provide: EVENT_API,
      useExisting: EventApiClient,
    },
  ];

  return makeEnvironmentProviders(providers);
}
