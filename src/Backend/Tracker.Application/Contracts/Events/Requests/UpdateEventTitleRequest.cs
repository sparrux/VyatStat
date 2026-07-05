namespace Tracker.Application.Contracts.Events.Requests;

public sealed record UpdateEventTitleRequest(
    string NewTitle
);