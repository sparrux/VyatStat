namespace Tracker.Application.Contracts.Groups.Responses;

public sealed record GroupSummaryResponse(
    Guid Id,
    string Name,
    int MemberCount
);