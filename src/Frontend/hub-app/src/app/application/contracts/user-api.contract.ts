import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';

import { ListResult } from '../models/list-result.model';
import { GetUsersQuery, UserSummary } from '../models/user-summary.model';

/**
 * Port for users API access.
 * Implemented by Infrastructure; consumed by Application services.
 */
export interface IUserApi {
  getList(query: GetUsersQuery): Observable<ListResult<UserSummary>>;
}

export const USER_API = new InjectionToken<IUserApi>('IUserApi');
