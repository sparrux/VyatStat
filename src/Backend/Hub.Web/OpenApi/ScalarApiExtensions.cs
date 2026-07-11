using OpenIddict.Abstractions;
using Scalar.AspNetCore;

namespace Hub.Web.OpenApi;

static class ScalarApiExtensions
{
    public static void MapScalar(this WebApplication app)
    {
        app.MapScalarApiReference((options, httpContext) =>
        {
            var authority = app.Configuration["OpenIddict:Authority"]?.TrimEnd('/');
            var audience = app.Configuration["OpenIddict:Audience"];
            var clientId = app.Configuration["Clients:hub-scalar:ClientId"];

            if (string.IsNullOrWhiteSpace(authority))
                throw new InvalidOperationException("OpenIddict:Authority is not configured.");

            if (string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("OpenIddict:Audience is not configured.");

            if (string.IsNullOrWhiteSpace(clientId))
                throw new InvalidOperationException("Clients:hub-scalar:ClientId is not configured.");

            var scalarBaseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/scalar/v1";

            options
                .AddPreferredSecuritySchemes("oauth2")
                .AddAuthorizationCodeFlow("oauth2", flow =>
                {
                    flow.ClientId = clientId;
                    flow.Pkce = Pkce.Sha256;
                    flow.RedirectUri = scalarBaseUrl;
                    flow.SelectedScopes =
                    [
                        OpenIddictConstants.Scopes.OpenId,
                        OpenIddictConstants.Scopes.Profile,
                        OpenIddictConstants.Scopes.OfflineAccess
                    ];

                    flow.WithCredentialsLocation(CredentialsLocation.Body);
                    flow.AddBodyParameter(OpenIddictConstants.Parameters.ClientId, clientId);
                    flow.AddBodyParameter(OpenIddictConstants.Claims.Audience, audience);
                    flow.AddQueryParameter(OpenIddictConstants.Parameters.RedirectUri, scalarBaseUrl);
                    flow.AddQueryParameter(OpenIddictConstants.Claims.Audience, audience);
                });
        });
    }
}