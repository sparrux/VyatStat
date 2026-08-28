import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';

import {
  CreateEventInput,
  CreateEventLocationInput,
  EventDetails,
  EventSummary,
} from '../models/event.model';
import { RichText } from '../models/rich-text.model';

/**
 * Port for events API access.
 * Implemented by Infrastructure; consumed by Application services.
 */
export interface IEventApi {
  create(input: CreateEventInput): Observable<EventSummary>;
  getById(eventId: string): Observable<EventDetails>;
  updateDescription(eventId: string, description: RichText): Observable<void>;
  updateLocation(
    eventId: string,
    location: CreateEventLocationInput,
  ): Observable<void>;
}

export const EVENT_API = new InjectionToken<IEventApi>('IEventApi');
