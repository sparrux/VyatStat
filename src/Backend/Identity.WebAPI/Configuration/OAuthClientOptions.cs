namespace Identity.WebAPI.Configuration;

public sealed class OAuthClientOptions
{
    public string ClientId { get; init; } = "";

    public string? Url { get; init; }

    public string Audience { get; init; } = "";
}
