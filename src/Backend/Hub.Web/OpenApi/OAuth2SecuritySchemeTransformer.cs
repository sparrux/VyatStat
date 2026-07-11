using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using OAuthOptions = Hub.Web.Authentication.OAuth.OAuthOptions;

namespace Hub.Web.OpenApi;

sealed class OAuth2SecuritySchemeTransformer(
    IConfiguration configuration
) : IOpenApiDocumentTransformer
{
    const string SecuritySchemeName = "oauth2";

    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var oAuthOptions = configuration
            .GetSection(OAuthOptions.SectionName)
            .Get<OAuthOptions>()!;

        oAuthOptions.EnsureValid();
        
        var scopes = oAuthOptions.Scopes
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToDictionary(x => x, _ => string.Empty);

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SecuritySchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"{oAuthOptions.Authority}/connect/authorize"),
                    TokenUrl = new Uri($"{oAuthOptions.Authority}/connect/token"),
                    Scopes = scopes
                }
            }
        };

        document.Security =
        [
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(SecuritySchemeName, document)] = scopes.Keys.ToList()
            }
        ];

        return Task.CompletedTask;
    }
}
