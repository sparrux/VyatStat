using Hub.Domain.Concepts.Requirements;

namespace Hub.Application.Features.Events.Contracts;

public sealed record EventRequirementSummaryResponse(
    Guid Id,
    string Title,
    string? Description
);