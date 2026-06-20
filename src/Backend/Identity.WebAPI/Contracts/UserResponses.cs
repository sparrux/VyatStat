namespace Identity.WebAPI.Contracts;

public sealed record UserResponse(
    Guid Id,
    string? UserName,
    string? Email,
    UserClaimsResponse? Claims,
    bool IsLockedOut
);

public sealed record UsersResponse(
    IReadOnlyCollection<UserResponse> Users,
    int Total
);