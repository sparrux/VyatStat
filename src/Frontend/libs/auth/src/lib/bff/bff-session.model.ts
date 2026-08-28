/** Matches Hub.Web `/auth/session` → `UserSummaryResponse`. */
export interface BffSessionUser {
  id: string;
  nickname: string;
}
