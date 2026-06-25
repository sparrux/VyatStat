namespace Identity.WebAPI.Contracts;

public sealed record LoginRequest(
    string Login,
    string Password
);

public sealed record AccountActionResponse(
    bool Success
);
