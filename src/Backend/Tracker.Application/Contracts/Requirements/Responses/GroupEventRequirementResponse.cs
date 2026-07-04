namespace Tracker.Application.Contracts.Requirements.Responses;

public sealed record GroupEventRequirementResponse(
    Guid Id,
    string Title,
    string? Description,
    bool IsMandatory
);