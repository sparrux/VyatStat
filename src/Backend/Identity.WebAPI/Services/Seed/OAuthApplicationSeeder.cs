using Identity.WebAPI.Configuration;
using OpenIddict.Abstractions;

namespace Identity.WebAPI.Services.Seed;

static class OAuthApplicationSeeder
{
    public static async Task SeedClientsAsync(WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        var clientRegistry = scope.ServiceProvider.GetRequiredService<IOAuthClientRegistry>();

        foreach (var client in clientRegistry.Clients)
        {
            if (string.IsNullOrWhiteSpace(client.Url))
                continue;

            var redirectUri = ResolveRedirectUri(client);
            var application = await manager.FindByClientIdAsync(client.ClientId);

            if (application is null)
            {
                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = client.ClientId,
                    ClientType = OpenIddictConstants.ClientTypes.Public,
                    ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
                    RedirectUris =
                    {
                        new(redirectUri)
                    },
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                        OpenIddictConstants.Permissions.ResponseTypes.Code,
                        OpenIddictConstants.Permissions.Scopes.Profile,

                        OpenIddictConstants.Scopes.OpenId,
                        OpenIddictConstants.Scopes.Profile,
                        OpenIddictConstants.Scopes.OfflineAccess
                    }
                };

                descriptor.AddAudiencePermissions(client.Audience);
                await manager.CreateAsync(descriptor);
                continue;
            }

            await EnsureRedirectUriAsync(manager, application, redirectUri);
            await EnsureAudiencePermissionAsync(manager, application, client.Audience);
        }
    }

    static string ResolveRedirectUri(OAuthClientOptions client)
    {
        if (!string.IsNullOrWhiteSpace(client.RedirectUri))
            return client.RedirectUri;

        return $"{client.Url!.TrimEnd('/')}/callback";
    }

    static async Task EnsureRedirectUriAsync(
        IOpenIddictApplicationManager manager,
        object application,
        string redirectUri)
    {
        var uris = await manager.GetRedirectUrisAsync(application);
        if (uris.Contains(redirectUri, StringComparer.Ordinal))
            return;

        var descriptor = new OpenIddictApplicationDescriptor();
        await manager.PopulateAsync(descriptor, application);
        descriptor.RedirectUris.Add(new Uri(redirectUri));
        await manager.UpdateAsync(application, descriptor);
    }

    static async Task EnsureAudiencePermissionAsync(
        IOpenIddictApplicationManager manager,
        object application,
        string audience)
    {
        var permissions = await manager.GetPermissionsAsync(application);
        var audiencePermission = OpenIddictConstants.Permissions.Prefixes.Audience + audience;

        if (permissions.Contains(audiencePermission, StringComparer.Ordinal))
            return;

        var descriptor = new OpenIddictApplicationDescriptor();
        await manager.PopulateAsync(descriptor, application);
        descriptor.AddAudiencePermissions(audience);
        await manager.UpdateAsync(application, descriptor);
    }
}
