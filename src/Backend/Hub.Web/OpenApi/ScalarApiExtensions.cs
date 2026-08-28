using Hub.Web.Authentication.OAuth;
using Scalar.AspNetCore;

namespace Hub.Web.OpenApi;

static class ScalarApiExtensions
{
    public static void MapScalar(this WebApplication app)
    {
        app.MapScalarApiReference((options, httpContext) =>
        {
            var authority = app.Configuration["OAuth:Authority"]?.TrimEnd('/');
            var audience = app.Configuration["OAuth:Audience"];
            var clientId = app.Configuration["Clients:hub-scalar:ClientId"];

            if (string.IsNullOrWhiteSpace(authority))
                throw new InvalidOperationException("OAuth:Authority is not configured.");

            if (string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("OAuth:Audience is not configured.");

            if (string.IsNullOrWhiteSpace(clientId))
                throw new InvalidOperationException("Clients:hub-scalar:ClientId is not configured.");

            var configuredRedirectUri = app.Configuration["Clients:hub-scalar:RedirectUri"];
            var scalarBaseUrl = !string.IsNullOrWhiteSpace(configuredRedirectUri)
                ? configuredRedirectUri
                : $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/scalar/v1";

            options
                .WithTitle("Vyatka Hub API")
                .AddPreferredSecuritySchemes("oauth2")
                .AddAuthorizationCodeFlow("oauth2", flow =>
                {
                    flow.ClientId = clientId;
                    flow.Pkce = Pkce.Sha256;
                    flow.RedirectUri = scalarBaseUrl;
                    flow.SelectedScopes =
                    [
                        OAuthConstants.Scopes.OpenId,
                        OAuthConstants.Scopes.Profile,
                        OAuthConstants.Scopes.OfflineAccess
                    ];

                    flow.WithCredentialsLocation(CredentialsLocation.Body);
                    flow.AddBodyParameter("client_id", clientId);
                    flow.AddBodyParameter(OAuthConstants.AudienceParameter, audience);
                    flow.AddQueryParameter("redirect_uri", scalarBaseUrl);
                    flow.AddQueryParameter(OAuthConstants.AudienceParameter, audience);
                });
        });
    }
}
