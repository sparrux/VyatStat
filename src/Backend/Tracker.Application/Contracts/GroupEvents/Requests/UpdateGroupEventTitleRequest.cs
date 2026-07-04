namespace Tracker.Application.Contracts.GroupEvents.Requests;

public sealed record UpdateGroupEventTitleRequest(
    string NewTitle
);