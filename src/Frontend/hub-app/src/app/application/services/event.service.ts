import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { EVENT_API } from '../contracts/event-api.contract';
import {
  CreateEventInput,
  CreateEventLocationInput,
  EventDetails,
  EventSummary,
} from '../models/event.model';
import { RichText } from '../models/rich-text.model';

/**
 * Application use-cases for events.
 * Presentation must depend on this service, not on Infrastructure.
 */
@Injectable({
  providedIn: 'root',
})
export class EventService {
  private readonly eventApi = inject(EVENT_API);

  create(input: CreateEventInput): Observable<EventSummary> {
    return this.eventApi.create(input);
  }

  getById(eventId: string): Observable<EventDetails> {
    return this.eventApi.getById(eventId);
  }

  updateDescription(eventId: string, description: RichText): Observable<void> {
    return this.eventApi.updateDescription(eventId, description);
  }

  updateLocation(
    eventId: string,
    location: CreateEventLocationInput,
  ): Observable<void> {
    return this.eventApi.updateLocation(eventId, location);
  }
}
