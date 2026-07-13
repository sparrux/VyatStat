using Hub.Domain.Events.Requirements;

namespace Hub.Application.Features.Events.Contracts;

public sealed record RequirementCompletionResponse(
    Guid Id,
    EventRequirementSummaryResponse Requirement,
    EventRequirementCompletionStatus CompletionStatus
);