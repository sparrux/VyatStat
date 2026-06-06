using OpenIddict.Abstractions;

namespace Identity.WebAPI.Services.Seed;

static class OAuthApplicationSeeder
{
    public static async Task SeedClientsAsync(WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
            
        var clientUrl = app.Configuration["Clients:WebClient:Url"];
        var clientId = app.Configuration["Clients:WebClient:ClientId"];
        
        if (await manager.FindByClientIdAsync("angular-client") is null) {
            await manager.CreateAsync(new()
            {
                ClientId = clientId,
                ClientType = OpenIddictConstants.ClientTypes.Public, // Public Client (SPA)
                ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
                RedirectUris =
                {
                    new($"{clientUrl}/callback")
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
            });
        }
    }
}