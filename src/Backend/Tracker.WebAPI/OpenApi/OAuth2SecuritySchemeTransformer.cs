using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using OpenIddict.Abstractions;

namespace Tracker.WebAPI.OpenApi;

sealed class OAuth2SecuritySchemeTransformer(IConfiguration configuration)
    : IOpenApiDocumentTransformer
{
    const string SecuritySchemeName = "oauth2";

    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authority = configuration["OpenIddict:Authority"]?.TrimEnd('/');
        var audience = configuration["OpenIddict:Audience"];

        if (string.IsNullOrWhiteSpace(authority))
            throw new InvalidOperationException("OpenIddict:Authority is not configured.");

        if (string.IsNullOrWhiteSpace(audience))
            throw new InvalidOperationException("OpenIddict:Audience is not configured.");

        var scopes = new Dictionary<string, string>
        {
            [OpenIddictConstants.Scopes.OpenId] = "Access the OpenID scope.",
            [OpenIddictConstants.Scopes.Profile] = "Access the profile scope.",
            [OpenIddictConstants.Scopes.OfflineAccess] = "Request refresh tokens."
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SecuritySchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"{authority}/connect/authorize"),
                    TokenUrl = new Uri($"{authority}/connect/token"),
                    Scopes = scopes
                }
            }
        };

        document.Security =
        [
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(SecuritySchemeName, document)] =
                [
                    OpenIddictConstants.Scopes.OpenId,
                    OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Scopes.OfflineAccess
                ]
            }
        ];

        return Task.CompletedTask;
    }
}
