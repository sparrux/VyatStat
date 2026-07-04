namespace Tracker.Application.Contracts.Requirements.Requests;

public sealed record UpdateGroupEventRequirementRequest(
    string Title, 
    string? Description, 
    bool IsMandatory
);