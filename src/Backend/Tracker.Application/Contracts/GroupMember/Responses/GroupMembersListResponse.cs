namespace Tracker.Application.Contracts.GroupMember.Responses;

public sealed record GroupMembersListResponse(
    IReadOnlyCollection<GroupMemberSummaryResponse> Members,
    int TotalCount
);