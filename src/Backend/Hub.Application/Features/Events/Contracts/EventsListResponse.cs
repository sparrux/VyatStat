namespace Hub.Application.Features.Events.Contracts;

public sealed record EventsListResponse(
    IReadOnlyCollection<EventSummaryResponse> Events,
    int TotalCount
);