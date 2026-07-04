namespace Tracker.Application.Contracts.GroupEvents.Responses;

public sealed record GroupEventsListResponse(
    IReadOnlyCollection<GroupEventSummaryResponse> Events,
    int TotalCount
);