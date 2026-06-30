namespace Tracker.Application.Contracts.User.Responses;

public sealed record UserSummaryResponse(
    Guid Id,
    string Nickname,
    DateTimeOffset CreatedAt
);