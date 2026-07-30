export interface UserSummary {
  id: string;
  nickname: string;
}

export interface GetUsersQuery {
  take: number;
  skip: number;
}
