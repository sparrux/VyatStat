namespace Tracker.Application.Contracts.Events.Requests;

public sealed record UpdateEventDatesRequest(
    DateTimeOffset NewEndDate,
    DateTimeOffset NewStartDate
);