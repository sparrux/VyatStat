import { TextFormat } from '../../../application/models/rich-text.model';
import { EventState } from '../../../application/models/event.model';

export interface RichTextDto {
  text: string;
  format: TextFormat;
}

export interface EventSummaryDto {
  id: string;
  title: string;
  state: EventState;
  endDate: string;
  startDate: string;
  hasLocation: boolean;
  inviteesCount: number;
  requirementsCount: number;
  goalsCount: number;
}

export interface EventLocationDto {
  id: string;
  name: string | null;
  latitude: number;
  longitude: number;
}

export interface EventRequirementSummaryDto {
  id: string;
  title: string;
  description: string | null;
}

export interface EventInviteeSummaryDto {
  id: string;
  user: {
    id: string;
    nickname: string;
  };
}

export interface EventDetailsDto {
  id: string;
  title: string;
  description: RichTextDto | null;
  endDate: string;
  startDate: string;
  state: EventState;
  location: EventLocationDto | null;
  requirements: EventRequirementSummaryDto[];
  invitees: EventInviteeSummaryDto[];
}
