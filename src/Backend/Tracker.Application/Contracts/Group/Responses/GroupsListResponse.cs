namespace Tracker.Application.Contracts.Group.Responses;

public sealed record GroupsListResponse(
    IReadOnlyCollection<GroupSummaryResponse> Groups,
    int TotalCount
);