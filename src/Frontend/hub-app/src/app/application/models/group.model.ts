export interface GroupSummary {
  id: string;
  name: string;
  membersCount: number;
}

export interface CreateGroupInput {
  name: string;
}

export interface GetGroupsQuery {
  memberUserId?: string;
  take: number;
  skip: number;
}
