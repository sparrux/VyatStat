namespace Hub.Application.Features.Users.Contracts;

public sealed record UserSummaryResponse(
    Guid Id,
    string Nickname
);