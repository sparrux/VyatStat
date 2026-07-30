import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';

import {
  CreateGroupInput,
  GetGroupsQuery,
  GroupSummary,
} from '../models/group.model';
import { ListResult } from '../models/list-result.model';

/**
 * Port for groups API access.
 * Implemented by Infrastructure; consumed by Application services.
 */
export interface IGroupApi {
  create(input: CreateGroupInput): Observable<GroupSummary>;
  getList(query: GetGroupsQuery): Observable<ListResult<GroupSummary>>;
}

export const GROUP_API = new InjectionToken<IGroupApi>('IGroupApi');
