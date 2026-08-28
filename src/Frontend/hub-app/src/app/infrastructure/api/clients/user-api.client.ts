import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { IUserApi } from '../../../application/contracts/user-api.contract';
import { ListResult } from '../../../application/models/list-result.model';
import {
  GetUsersQuery,
  UserSummary,
} from '../../../application/models/user-summary.model';
import { environment } from '../../../../environments/environment';
import { ListResponseDto } from '../dto/list-response.dto';
import { UserSummaryDto } from '../dto/user.dto';
import { mapUserListResponseDtoToModel } from '../mappers/user-summary.mapper';

/**
 * Infrastructure adapter for Hub Users REST API.
 */
@Injectable()
export class UserApiClient implements IUserApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.hubApiUrl}/api/v1/users`;

  getList(query: GetUsersQuery): Observable<ListResult<UserSummary>> {
    const params = new HttpParams()
      .set('take', query.take)
      .set('skip', query.skip);

    return this.http
      .get<ListResponseDto<UserSummaryDto>>(`${this.baseUrl}/`, { params })
      .pipe(map(mapUserListResponseDtoToModel));
  }
}
