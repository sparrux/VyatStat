namespace Identity.WebAPI.Contracts;

public sealed record RegistrationRequest(
    string Login, 
    string Password
);

public sealed record UpdatePasswordRequest(
    string CurrentPassword,
    string NewPassword
);