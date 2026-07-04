using Tracker.Application.Contracts.Groups.Responses;

namespace Tracker.Application.Contracts.Users.Responses;

public sealed record UserDetailsResponse(
    Guid Id,
    string Nickname,
    IReadOnlyCollection<GroupSummaryResponse> Groups,
    DateTimeOffset CreatedAt
);