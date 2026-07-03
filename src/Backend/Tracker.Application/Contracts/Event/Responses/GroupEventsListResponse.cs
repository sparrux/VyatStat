namespace Tracker.Application.Contracts.Event.Responses;

public sealed record GroupEventsListResponse(
    IReadOnlyCollection<GroupEventSummaryResponse> Events,
    int TotalCount
);