namespace Hub.Web.Authentication.OAuth;

static class OAuthConstants
{
    public const string CookieName = "vyatka.hub.session";
    public const string SmartAuthScheme = "Smart";
    public const string AudienceParameter = "aud";

    public static class Scopes
    {
        public const string OpenId = "openid";
        public const string Profile = "profile";
        public const string OfflineAccess = "offline_access";
    }

    public static class Claims
    {
        public const string Subject = "sub";
        public const string Username = "username";
        public const string SessionId = "sid";
    }
}
