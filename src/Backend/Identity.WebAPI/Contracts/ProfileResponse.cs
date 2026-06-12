namespace Identity.WebAPI.Contracts;

public sealed record ProfileResponse(
    Guid Id,
    string? UserName
);