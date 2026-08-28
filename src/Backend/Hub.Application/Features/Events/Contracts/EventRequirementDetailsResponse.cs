using Hub.Domain.Events.Requirements;

namespace Hub.Application.Features.Events.Contracts;

public sealed record EventRequirementDetailsResponse(
    Guid Id,
    Guid EventId,
    string Title,
    string? Description,
    RequirementAssignmentPolicy AssignmentPolicy,
    IReadOnlyCollection<EventRequirementVerifierDetailsResponse> Verifiers
);