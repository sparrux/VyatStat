import { RichText } from './rich-text.model';
import { UserSummary } from './user-summary.model';

export enum EventState {
  Draft = 0,
  RegistrationOpen = 1,
  RegistrationClosed = 2,
  InProgress = 3,
  Completed = 4,
  Cancelled = 5,
}

export interface EventSummary {
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

export interface EventLocation {
  id: string;
  name: string | null;
  latitude: number;
  longitude: number;
}

export interface EventRequirementSummary {
  id: string;
  title: string;
  description: string | null;
}

export interface EventInviteeSummary {
  id: string;
  user: UserSummary;
}

export interface EventDetails {
  id: string;
  title: string;
  description: RichText | null;
  endDate: string;
  startDate: string;
  state: EventState;
  location: EventLocation | null;
  requirements: EventRequirementSummary[];
  invitees: EventInviteeSummary[];
}

export interface GetGroupEventsQuery {
  groupId: string;
  fromDate: string;
  toDate: string;
}

export interface DatesRange {
  startDate: string;
  endDate: string;
}

export interface CreateEventLocationInput {
  name?: string | null;
  latitude: number;
  longitude: number;
}

export interface CreateEventInput {
  title: string;
  startDate: string;
  endDate: string;
  description?: string | null;
  location?: CreateEventLocationInput | null;
}

export interface CreateEventDraftResult {
  event: EventSummary;
  groupId: string;
}
