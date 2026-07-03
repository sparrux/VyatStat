namespace Tracker.Application.Contracts.Event.Requests;

public sealed record UpdateGroupEventRequirementRequest(
    string Title, 
    string? Description, 
    bool IsMandatory
);