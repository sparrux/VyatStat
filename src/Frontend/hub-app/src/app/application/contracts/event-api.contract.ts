import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';

import { EventDetails } from '../models/event.model';

/**
 * Port for events API access.
 * Implemented by Infrastructure; consumed by Application services.
 */
export interface IEventApi {
  getById(eventId: string): Observable<EventDetails>;
}

export const EVENT_API = new InjectionToken<IEventApi>('IEventApi');
