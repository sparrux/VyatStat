import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { GROUP_API } from '../contracts/group-api.contract';
import {
  CreateGroupInput,
  GetGroupsQuery,
  GroupSummary,
} from '../models/group.model';
import {
  EventSummary,
  GetGroupEventsQuery,
} from '../models/event.model';
import { ListResult } from '../models/list-result.model';

/**
 * Application use-cases for groups.
 * Presentation must depend on this service, not on Infrastructure.
 */
@Injectable({
  providedIn: 'root',
})
export class GroupService {
  private readonly groupApi = inject(GROUP_API);

  readonly pageSize = 10;

  create(input: CreateGroupInput): Observable<GroupSummary> {
    return this.groupApi.create(input);
  }

  getList(query: GetGroupsQuery): Observable<ListResult<GroupSummary>> {
    return this.groupApi.getList(query);
  }

  getEvents(query: GetGroupEventsQuery): Observable<ListResult<EventSummary>> {
    return this.groupApi.getEvents(query);
  }
}
