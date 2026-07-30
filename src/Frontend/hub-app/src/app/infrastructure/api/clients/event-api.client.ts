import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { IEventApi } from '../../../application/contracts/event-api.contract';
import { EventDetails } from '../../../application/models/event.model';
import { environment } from '../../../../environments/environment';
import { EventDetailsDto } from '../dto/event.dto';
import { mapEventDetailsDtoToModel } from '../mappers/event.mapper';

/**
 * Infrastructure adapter for Hub Events REST API.
 */
@Injectable()
export class EventApiClient implements IEventApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.hubApiUrl}/api/v1/events`;

  getById(eventId: string): Observable<EventDetails> {
    return this.http
      .get<EventDetailsDto>(`${this.baseUrl}/${eventId}`)
      .pipe(map(mapEventDetailsDtoToModel));
  }
}
