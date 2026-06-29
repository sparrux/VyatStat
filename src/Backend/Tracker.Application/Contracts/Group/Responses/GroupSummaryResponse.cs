namespace Tracker.Application.Contracts.Group.Responses;

public sealed class GroupSummaryResponse(
    Guid Id,
    string Name,
    int MemberCount
);