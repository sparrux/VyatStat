import { ListResult } from '../../../application/models/list-result.model';
import { UserSummary } from '../../../application/models/user-summary.model';
import { ListResponseDto } from '../dto/list-response.dto';
import { UserSummaryDto } from '../dto/user.dto';

export function mapUserSummaryDtoToModel(dto: UserSummaryDto): UserSummary {
  return {
    id: dto.id,
    nickname: dto.nickname,
  };
}

export function mapUserListResponseDtoToModel(
  dto: ListResponseDto<UserSummaryDto>,
): ListResult<UserSummary> {
  return {
    values: dto.values.map(mapUserSummaryDtoToModel),
    total: dto.total,
  };
}
