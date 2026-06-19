namespace Identity.WebAPI.Authentication;

static class UserClaimTypes
{
    public const string Role = "id.user.role";
    public const string Permission = "id.user.permission";
    public const string SecurityStamp = "id.user.security_stamp";
}

static class UserClaims
{
    // Roles
    public const string Admin = "id.user.role.admin";
    
    // Permissions
    public const string CanReadUsers = "id.user.permission.read_users";
    public const string CanUpdateUserPermissions = "id.user.permission.update_user_permissions";
}