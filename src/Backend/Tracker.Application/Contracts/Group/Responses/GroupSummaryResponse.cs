namespace Tracker.Application.Contracts.Group.Responses;

public sealed record GroupSummaryResponse(
    Guid Id,
    string Name,
    int MemberCount
);