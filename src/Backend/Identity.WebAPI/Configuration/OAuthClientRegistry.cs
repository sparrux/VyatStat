namespace Identity.WebAPI.Configuration;

sealed class OAuthClientRegistry : IOAuthClientRegistry
{
    readonly OAuthClientOptions[] _clients;
    readonly Dictionary<string, OAuthClientOptions> _byClientId;

    public OAuthClientRegistry(IConfiguration configuration)
    {
        _clients = LoadClients(configuration);
        _byClientId = _clients.ToDictionary(client => client.ClientId, StringComparer.Ordinal);
    }

    public IReadOnlyCollection<OAuthClientOptions> Clients => _clients;

    public OAuthClientOptions? FindByClientId(string clientId) =>
        _byClientId.GetValueOrDefault(clientId);

    static OAuthClientOptions[] LoadClients(IConfiguration configuration)
    {
        var clients = new List<OAuthClientOptions>();

        foreach (var section in configuration.GetSection("Clients").GetChildren())
        {
            var client = section.Get<OAuthClientOptions>();
            if (client is null || string.IsNullOrWhiteSpace(client.ClientId))
                continue;

            clients.Add(client);
        }

        return clients.ToArray();
    }
}
