using Tracker.Domain.GroupEvents.Invitees;

namespace Tracker.Application.Contracts.Requirements.Responses;

public sealed record InviteeRequirementCompletionResponse(
    GroupEventRequirementResponse Requirement,
    GroupEventInviteeRequirementStatus Status
);