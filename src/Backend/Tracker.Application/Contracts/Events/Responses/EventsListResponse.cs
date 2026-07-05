namespace Tracker.Application.Contracts.Events.Responses;

public sealed record EventsListResponse(
    IReadOnlyCollection<EventSummaryResponse> Events,
    int TotalCount
);