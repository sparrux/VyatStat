namespace Identity.WebAPI.Exceptions;

static class ApiErrors
{
    public const string UnexpectedError = "An unexpected error occurred. Please try again later";
    public const string UserNotFound = "User not found";
    public const string FailedToUpdatePermissions = "Failed to update user permissions";
    public const string InvalidUserIdentifier = "Invalid user identifier";

    public static class OAuth
    {
        public const string InvalidAudience = "Invalid audience";
        public const string UnsupportedGrantType = "Current grand type is unsupported";
        public const string InvalidClient = "The client application not found by client id";
        public const string InvalidRequest = "Invalid OAuth 2.0 request";
        public const string AccountLockedOut = "Your account is locked out";
        public const string InvalidUserCredentials = "Invalid login or password";
    }
}
