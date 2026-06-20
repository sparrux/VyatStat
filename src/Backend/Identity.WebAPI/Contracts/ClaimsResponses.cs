namespace Identity.WebAPI.Contracts;

public record UserClaimsResponse(
    bool IsAdmin,
    bool ReadUsers,
    bool UpdateUserPermissions,
    bool LockOutUsers
);
