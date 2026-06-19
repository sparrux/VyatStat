namespace Identity.WebAPI.Contracts;

public record UpdateUserPermissionsRequest(
    bool? ReadUsers = null,
    bool? UpdateUserPermissions = null,
    bool? LockOutUsers = null
);