using Tracker.Application.Contracts.Group.Responses;

namespace Tracker.Application.Contracts.User.Responses;

public sealed record UserDetailsResponse(
    Guid Id,
    string Nickname,
    IReadOnlyCollection<GroupSummaryResponse> Groups,
    DateTimeOffset CreatedAt
);