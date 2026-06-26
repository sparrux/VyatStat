export interface UserProfile {
  id: string;
  userName: string | null;
  email: string | null;
  claims: UserClaims | null;
}

export interface UserClaims {
  isAdmin: boolean;
  readUsers: boolean;
  updateUserPermissions: boolean;
  lockOutUsers: boolean;
}

export interface OAuthTokenResponse {
  access_token: string;
  refresh_token?: string;
  expires_in?: number;
}

export interface UpdatePasswordRequest {
  currentPassword: string;
  newPassword: string;
}
