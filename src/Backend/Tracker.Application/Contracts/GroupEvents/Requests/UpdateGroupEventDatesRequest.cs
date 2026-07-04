namespace Tracker.Application.Contracts.GroupEvents.Requests;

public sealed record UpdateGroupEventDatesRequest(
    DateTimeOffset NewEndDate,
    DateTimeOffset NewStartDate
);