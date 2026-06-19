using OpenIddict.Abstractions;

namespace Identity.WebAPI.Authentication;

public interface IAudienceResolver
{
    IReadOnlyList<string> AllowedAudiences { get; }

    bool IsAllowed(string audience);

    string? ResolveFromTokenRequest(OpenIddictRequest request, string? clientId = null);
}
