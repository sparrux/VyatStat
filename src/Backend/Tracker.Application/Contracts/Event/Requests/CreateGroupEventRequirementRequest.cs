namespace Tracker.Application.Contracts.Event.Requests;

public sealed record CreateGroupEventRequirementRequest(
    string Title, 
    string? Description, 
    bool IsMandatory
);