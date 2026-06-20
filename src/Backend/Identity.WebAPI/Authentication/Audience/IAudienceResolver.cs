using OpenIddict.Abstractions;

namespace Identity.WebAPI.Authentication.Audience;

public interface IAudienceResolver
{
    IReadOnlyList<string> AllowedAudiences { get; }

    bool IsAllowed(string audience);

    string? ResolveFromTokenRequest(OpenIddictRequest request, string? clientId = null);
}
