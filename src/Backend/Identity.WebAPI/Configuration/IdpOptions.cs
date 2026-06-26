namespace Identity.WebAPI.Configuration;

public sealed class IdpOptions
{
    public const string SectionName = "Idp";

    /// <summary>Absolute URL of the IdP login page (identity-app /login).</summary>
    public string LoginPageUrl { get; set; } = "";

    /// <summary>Identity Server public base URL used to validate authorize return URLs.</summary>
    public string Authority { get; set; } = "";
}
