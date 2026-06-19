using Identity.WebAPI.Configuration;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;

namespace Identity.WebAPI.Authentication;

sealed class AudienceResolver(
    IOptions<OpenIddictOptions> openIddictOptions,
    IOAuthClientRegistry clientRegistry
) : IAudienceResolver
{
    readonly HashSet<string> _allowedAudiences = openIddictOptions.Value.Audiences
        .ToHashSet(StringComparer.Ordinal);

    public IReadOnlyList<string> AllowedAudiences => openIddictOptions.Value.Audiences;

    public bool IsAllowed(string audience) =>
        !string.IsNullOrWhiteSpace(audience) && _allowedAudiences.Contains(audience);

    public string? ResolveFromTokenRequest(OpenIddictRequest request, string? clientId = null)
    {
        var customAudience = request.GetParameter(OpenIddictConstants.Claims.Audience)?.ToString();
        if (customAudience is not null)
            return IsAllowed(customAudience) ? customAudience : null;

        foreach (var audience in request.GetAudiences())
        {
            if (!IsAllowed(audience))
                return null;

            return audience;
        }

        if (clientId is not null
            && clientRegistry.FindByClientId(clientId) is { Audience: var configuredAudience }
            && IsAllowed(configuredAudience))
            return configuredAudience;

        return null;
    }
}
