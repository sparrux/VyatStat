import { UserClaims } from '@vyatka-tracker/auth';

export interface DashboardUser {
  id: string;
  userName: string | null;
  email: string | null;
  claims: UserClaims | null;
  isLockedOut: boolean;
}

export interface UsersListResponse {
  users: DashboardUser[];
  total: number;
}

export interface UpdateUserPermissionsRequest {
  readUsers: boolean;
  updateUserPermissions: boolean;
  lockOutUsers: boolean;
}
