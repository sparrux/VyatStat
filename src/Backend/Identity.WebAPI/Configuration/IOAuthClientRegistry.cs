namespace Identity.WebAPI.Configuration;

public interface IOAuthClientRegistry
{
    IReadOnlyCollection<OAuthClientOptions> Clients { get; }

    OAuthClientOptions? FindByClientId(string clientId);
}
