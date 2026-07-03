namespace Tracker.Application.Contracts.Event.Requests;

public sealed record UpdateGroupEventTitleRequest(
    string NewTitle
);