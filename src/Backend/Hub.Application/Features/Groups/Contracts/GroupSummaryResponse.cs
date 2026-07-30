namespace Hub.Application.Features.Groups.Contracts;

public sealed record GroupSummaryResponse(
    Guid Id,
    string Name,
    int MembersCount
);