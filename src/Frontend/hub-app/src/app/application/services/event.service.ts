import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { EVENT_API } from '../contracts/event-api.contract';
import { EventDetails } from '../models/event.model';

/**
 * Application use-cases for events.
 * Presentation must depend on this service, not on Infrastructure.
 */
@Injectable({
  providedIn: 'root',
})
export class EventService {
  private readonly eventApi = inject(EVENT_API);

  getById(eventId: string): Observable<EventDetails> {
    return this.eventApi.getById(eventId);
  }
}
