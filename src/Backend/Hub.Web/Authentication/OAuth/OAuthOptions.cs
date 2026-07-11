namespace Hub.Web.Authentication.OAuth;

sealed class OAuthOptions
{
    public const string SectionName = "OAuth";

    public string Authority { get; init; } = "";
    public string Audience { get; init; } = "";
    public string ClientId { get; init; } = "";
    public string CallbackPath { get; init; } = "/auth/callback";
    public string Scopes { get; init; } = "openid profile offline_access";
    public string DefaultReturnUrl { get; init; } = "/";

    public TimeSpan TokenLifetime { get; init; } = TimeSpan.FromDays(30);
    
    public void EnsureValid()
    {
        if (string.IsNullOrWhiteSpace(Authority))
            throw new InvalidOperationException($"{SectionName}:{nameof(Authority)} is not configured.");
        if (string.IsNullOrWhiteSpace(Audience))
            throw new InvalidOperationException($"{SectionName}:{nameof(Audience)} is not configured.");
        if (string.IsNullOrWhiteSpace(ClientId))
            throw new InvalidOperationException($"{SectionName}:{nameof(ClientId)} is not configured.");
    }
}
