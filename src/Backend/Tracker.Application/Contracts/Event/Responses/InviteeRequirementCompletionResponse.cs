namespace Tracker.Application.Contracts.Event.Responses;

public sealed record InviteeRequirementCompletionResponse(
    GroupEventRequirementResponse Requirement
);