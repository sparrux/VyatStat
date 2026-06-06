namespace Identity.WebAPI.Contracts;

public record RegistrationRequest(
    string Login, 
    string Password
);

public record LoginRequest(
    string Login, 
    string Password
);