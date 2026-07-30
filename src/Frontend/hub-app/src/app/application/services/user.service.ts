import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { USER_API } from '../contracts/user-api.contract';
import { ListResult } from '../models/list-result.model';
import { GetUsersQuery, UserSummary } from '../models/user-summary.model';

/**
 * Application use-cases for users.
 * Presentation must depend on this service, not on Infrastructure.
 */
@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly userApi = inject(USER_API);

  readonly pageSize = 10;

  getList(query: GetUsersQuery): Observable<ListResult<UserSummary>> {
    return this.userApi.getList(query);
  }
}
