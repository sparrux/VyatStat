using Tracker.Domain.Events.Requirements;

namespace Tracker.Application.Contracts.Requirements.Responses;

public sealed record RequirementCompletionResponse(
    Guid Id,
    EventRequirementResponse Requirement,
    EventRequirementCompletionStatus CompletionStatus
);