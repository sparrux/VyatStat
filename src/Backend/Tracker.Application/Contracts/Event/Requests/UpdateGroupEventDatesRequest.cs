namespace Tracker.Application.Contracts.Event.Requests;

public sealed record UpdateGroupEventDatesRequest(
    DateTimeOffset NewEndDate,
    DateTimeOffset NewStartDate
);