namespace Identity.WebAPI.Contracts;

public record UserPermissionsResponse(
    bool IsAdmin,
    bool ReadUsers,
    bool UpdateUserPermissions
);
