namespace Tracker.Application.Contracts.Groups.Responses;

public sealed record GroupsListResponse(
    IReadOnlyCollection<GroupSummaryResponse> Groups,
    int TotalCount
);