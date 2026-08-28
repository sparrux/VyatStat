import {
  EventDetails,
  EventInviteeSummary,
  EventLocation,
  EventRequirementSummary,
  EventSummary,
} from '../../../application/models/event.model';
import { ListResult } from '../../../application/models/list-result.model';
import { RichText } from '../../../application/models/rich-text.model';
import {
  EventDetailsDto,
  EventInviteeSummaryDto,
  EventLocationDto,
  EventRequirementSummaryDto,
  EventSummaryDto,
  RichTextDto,
} from '../dto/event.dto';
import { ListResponseDto } from '../dto/list-response.dto';

export function mapRichTextDtoToModel(dto: RichTextDto): RichText {
  return {
    text: dto.text,
    format: dto.format,
  };
}

export function mapEventSummaryDtoToModel(dto: EventSummaryDto): EventSummary {
  return {
    id: dto.id,
    title: dto.title,
    state: dto.state,
    endDate: dto.endDate,
    startDate: dto.startDate,
    hasLocation: dto.hasLocation,
    inviteesCount: dto.inviteesCount,
    requirementsCount: dto.requirementsCount,
    goalsCount: dto.goalsCount,
  };
}

export function mapEventListResponseDtoToModel(
  dto: ListResponseDto<EventSummaryDto>,
): ListResult<EventSummary> {
  return {
    values: dto.values.map(mapEventSummaryDtoToModel),
    total: dto.total,
  };
}

export function mapEventLocationDtoToModel(
  dto: EventLocationDto,
): EventLocation {
  return {
    id: dto.id,
    name: dto.name,
    latitude: dto.latitude,
    longitude: dto.longitude,
  };
}

export function mapEventRequirementSummaryDtoToModel(
  dto: EventRequirementSummaryDto,
): EventRequirementSummary {
  return {
    id: dto.id,
    title: dto.title,
    description: dto.description,
  };
}

export function mapEventInviteeSummaryDtoToModel(
  dto: EventInviteeSummaryDto,
): EventInviteeSummary {
  return {
    id: dto.id,
    user: {
      id: dto.user.id,
      nickname: dto.user.nickname,
    },
  };
}

export function mapEventDetailsDtoToModel(dto: EventDetailsDto): EventDetails {
  return {
    id: dto.id,
    title: dto.title,
    description: dto.description
      ? mapRichTextDtoToModel(dto.description)
      : null,
    endDate: dto.endDate,
    startDate: dto.startDate,
    state: dto.state,
    location: dto.location ? mapEventLocationDtoToModel(dto.location) : null,
    requirements: dto.requirements.map(mapEventRequirementSummaryDtoToModel),
    invitees: dto.invitees.map(mapEventInviteeSummaryDtoToModel),
  };
}
