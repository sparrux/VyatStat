namespace Identity.WebAPI.Contracts;

public record RegistrationRequest(
    string Login, 
    string Password
);