namespace Tracker.Application.Contracts.Requirements.Requests;

public sealed record CreateGroupEventRequirementRequest(
    string Title, 
    string? Description, 
    bool IsMandatory
);