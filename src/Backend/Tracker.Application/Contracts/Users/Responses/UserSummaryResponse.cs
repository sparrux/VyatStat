namespace Tracker.Application.Contracts.Users.Responses;

public sealed record UserSummaryResponse(
    Guid Id,
    string Nickname,
    DateTimeOffset CreatedAt
);