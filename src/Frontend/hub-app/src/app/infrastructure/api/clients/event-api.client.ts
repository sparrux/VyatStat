import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { IEventApi } from '../../../application/contracts/event-api.contract';
import {
  CreateEventInput,
  CreateEventLocationInput,
  EventDetails,
  EventSummary,
} from '../../../application/models/event.model';
import { RichText } from '../../../application/models/rich-text.model';
import { environment } from '../../../../environments/environment';
import {
  CreateEventRequestDto,
  EventDetailsDto,
  EventSummaryDto,
  IdResponseDto,
  UpdateDescriptionRequestDto,
  UpdateLocationRequestDto,
} from '../dto/event.dto';
import {
  mapEventDetailsDtoToModel,
  mapEventSummaryDtoToModel,
} from '../mappers/event.mapper';

/**
 * Infrastructure adapter for Hub Events REST API.
 */
@Injectable()
export class EventApiClient implements IEventApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.hubApiUrl}/api/v1/events`;

  create(input: CreateEventInput): Observable<EventSummary> {
    const body: CreateEventRequestDto = {
      title: input.title,
      dates: {
        startDate: input.startDate,
        endDate: input.endDate,
      },
    };

    return this.http
      .post<EventSummaryDto>(`${this.baseUrl}/`, body)
      .pipe(map(mapEventSummaryDtoToModel));
  }

  getById(eventId: string): Observable<EventDetails> {
    return this.http
      .get<EventDetailsDto>(`${this.baseUrl}/${eventId}`)
      .pipe(map(mapEventDetailsDtoToModel));
  }

  updateDescription(eventId: string, description: RichText): Observable<void> {
    const body: UpdateDescriptionRequestDto = {
      newDescription: {
        text: description.text,
        format: description.format,
      },
    };

    return this.http
      .put<IdResponseDto>(`${this.baseUrl}/${eventId}/description`, body)
      .pipe(map(() => undefined));
  }

  updateLocation(
    eventId: string,
    location: CreateEventLocationInput,
  ): Observable<void> {
    const body: UpdateLocationRequestDto = {
      name: location.name ?? null,
      latitude: location.latitude,
      longitude: location.longitude,
    };

    return this.http
      .put<IdResponseDto>(`${this.baseUrl}/${eventId}/location`, body)
      .pipe(map(() => undefined));
  }
}
