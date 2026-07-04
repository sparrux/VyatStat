namespace Tracker.Application.Contracts.GroupMembers.Responses;

public sealed record GroupMembersListResponse(
    IReadOnlyCollection<GroupMemberSummaryResponse> Members,
    int TotalCount
);