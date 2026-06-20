import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, defer, switchMap, throwError } from 'rxjs';
import { UserClaims } from '../models/auth.model';
import {
  DashboardUser,
  UpdateUserPermissionsRequest,
  UsersListResponse,
} from '../models/user.model';
import { AuthService } from './auth.service';

const PAGE_SIZE = 10;

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  private readonly auth = inject(AuthService);
  private readonly http = inject(HttpClient);

  readonly pageSize = PAGE_SIZE;

  getUsers(skip = 0, take = PAGE_SIZE): Observable<UsersListResponse> {
    return defer(() => this.auth.ensureAccessTokenIfNeeded()).pipe(
      switchMap(() => {
        const token = localStorage.getItem('access_token');
        if (!token) {
          return throwError(() => new Error('Missing access token after refresh'));
        }

        const headers = new HttpHeaders({
          Authorization: `Bearer ${token}`,
        });
        const params = new HttpParams()
          .set('take', take.toString())
          .set('skip', skip.toString());

        return this.http.get<UsersListResponse>(`${this.auth.getAuthServerUrl()}/users`, {
          headers,
          params,
        });
      }),
    );
  }

  updateUserPermissions(
    userId: string,
    request: UpdateUserPermissionsRequest,
  ): Observable<UserClaims> {
    return defer(() => this.auth.ensureAccessTokenIfNeeded()).pipe(
      switchMap(() => {
        const token = localStorage.getItem('access_token');
        if (!token) {
          return throwError(() => new Error('Missing access token after refresh'));
        }

        const headers = new HttpHeaders({
          Authorization: `Bearer ${token}`,
        });

        return this.http.post<UserClaims>(
          `${this.auth.getAuthServerUrl()}/users/${userId}/permissions`,
          request,
          { headers },
        );
      }),
    );
  }

  setUserLockOut(userId: string, lockout: boolean): Observable<void> {
    return defer(() => this.auth.ensureAccessTokenIfNeeded()).pipe(
      switchMap(() => {
        const token = localStorage.getItem('access_token');
        if (!token) {
          return throwError(() => new Error('Missing access token after refresh'));
        }

        const headers = new HttpHeaders({
          Authorization: `Bearer ${token}`,
        });
        const params = new HttpParams().set('lockout', String(lockout));

        return this.http.put<void>(
          `${this.auth.getAuthServerUrl()}/users/${userId}/lock`,
          null,
          { headers, params },
        );
      }),
    );
  }
}
