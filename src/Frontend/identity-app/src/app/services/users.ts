import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, defer, of, switchMap, throwError } from 'rxjs';
import { AuthService, UserClaims } from './auth';

export interface DashboardUser {
  id: string;
  userName: string | null;
  email: string | null;
  claims: UserClaims | null;
}

export interface UsersListResponse {
  users: DashboardUser[];
  total: number;
}

export interface UpdateUserPermissionsRequest {
  readUsers: boolean;
  updateUserPermissions: boolean;
}

const PAGE_SIZE = 30;

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  private readonly auth = inject(AuthService);
  private readonly http = inject(HttpClient);

  readonly pageSize = PAGE_SIZE;

  getUsers(skip = 0, take = PAGE_SIZE): Observable<UsersListResponse> {
    return defer(() => this.ensureAccessTokenIfNeeded()).pipe(
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
    return defer(() => this.ensureAccessTokenIfNeeded()).pipe(
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

  private ensureAccessTokenIfNeeded(): Observable<void> {
    if (localStorage.getItem('access_token')) {
      return of(undefined);
    }
    if (!localStorage.getItem('refresh_token')) {
      return throwError(() => new Error('Missing refresh token'));
    }
    return this.auth.refreshAccessTokenSilently();
  }
}
