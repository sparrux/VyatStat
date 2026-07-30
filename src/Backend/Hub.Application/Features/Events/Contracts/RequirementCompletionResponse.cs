namespace Hub.Application.Features.Events.Contracts;

public sealed record RequirementCompletionResponse(
    Guid Id,
    EventRequirementSummaryResponse Requirement
);