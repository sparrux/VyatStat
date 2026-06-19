namespace Identity.WebAPI.Configuration;

public sealed class OpenIddictOptions
{
    public const string SectionName = "OpenIddict";

    public string[] Audiences { get; init; } = [];
}
