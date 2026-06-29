namespace Tracker.Application.Contracts.User.Responses;

public sealed class UserSummaryResponse(
    Guid Id,
    string Nickname,
    DateTimeOffset CreatedAt
);