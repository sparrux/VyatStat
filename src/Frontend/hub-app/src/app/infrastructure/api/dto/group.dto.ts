export interface GroupSummaryDto {
  id: string;
  name: string;
  membersCount: number;
}

export interface CreateGroupRequestDto {
  name: string;
}

export interface GetGroupsQueryDto {
  memberUserId?: string;
  take: number;
  skip: number;
}
