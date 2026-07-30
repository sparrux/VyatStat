import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { IGroupApi } from '../../../application/contracts/group-api.contract';
import {
  CreateGroupInput,
  GetGroupsQuery,
  GroupSummary,
} from '../../../application/models/group.model';
import {
  EventSummary,
  GetGroupEventsQuery,
} from '../../../application/models/event.model';
import { ListResult } from '../../../application/models/list-result.model';
import { environment } from '../../../../environments/environment';
import {
  CreateGroupRequestDto,
  GroupSummaryDto,
} from '../dto/group.dto';
import { EventSummaryDto } from '../dto/event.dto';
import { ListResponseDto } from '../dto/list-response.dto';
import {
  mapGroupListResponseDtoToModel,
  mapGroupSummaryDtoToModel,
} from '../mappers/group.mapper';
import { mapEventListResponseDtoToModel } from '../mappers/event.mapper';

/**
 * Infrastructure adapter for Hub Groups REST API.
 */
@Injectable()
export class GroupApiClient implements IGroupApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.hubApiUrl}/api/v1/groups`;

  create(input: CreateGroupInput): Observable<GroupSummary> {
    const body: CreateGroupRequestDto = { name: input.name };

    return this.http
      .post<GroupSummaryDto>(`${this.baseUrl}/`, body)
      .pipe(map(mapGroupSummaryDtoToModel));
  }

  getList(query: GetGroupsQuery): Observable<ListResult<GroupSummary>> {
    let params = new HttpParams()
      .set('take', query.take)
      .set('skip', query.skip);

    if (query.memberUserId) {
      params = params.set('memberUserId', query.memberUserId);
    }

    return this.http
      .get<ListResponseDto<GroupSummaryDto>>(`${this.baseUrl}/`, { params })
      .pipe(map(mapGroupListResponseDtoToModel));
  }

  getEvents(query: GetGroupEventsQuery): Observable<ListResult<EventSummary>> {
    const params = new HttpParams()
      .set('groupId', query.groupId)
      .set('fromDate', query.fromDate)
      .set('toDate', query.toDate);

    return this.http
      .get<ListResponseDto<EventSummaryDto>>(`${this.baseUrl}/events`, {
        params,
      })
      .pipe(map(mapEventListResponseDtoToModel));
  }
}
