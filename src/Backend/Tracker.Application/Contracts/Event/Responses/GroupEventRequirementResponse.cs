namespace Tracker.Application.Contracts.Event.Responses;

public sealed record GroupEventRequirementResponse(
    Guid Id,
    string Title,
    string? Description,
    bool IsMandatory
);