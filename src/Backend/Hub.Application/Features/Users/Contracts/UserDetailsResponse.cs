namespace Hub.Application.Features.Users.Contracts;

public sealed record UserDetailsResponse(
    Guid Id,
    string Nickname
);