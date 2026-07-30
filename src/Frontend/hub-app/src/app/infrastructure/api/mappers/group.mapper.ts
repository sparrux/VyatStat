import { GroupSummary } from '../../../application/models/group.model';
import { ListResult } from '../../../application/models/list-result.model';
import { GroupSummaryDto } from '../dto/group.dto';
import { ListResponseDto } from '../dto/list-response.dto';

export function mapGroupSummaryDtoToModel(dto: GroupSummaryDto): GroupSummary {
  return {
    id: dto.id,
    name: dto.name,
    membersCount: dto.membersCount,
  };
}

export function mapGroupListResponseDtoToModel(
  dto: ListResponseDto<GroupSummaryDto>,
): ListResult<GroupSummary> {
  return {
    values: dto.values.map(mapGroupSummaryDtoToModel),
    total: dto.total,
  };
}
