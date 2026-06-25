using Identity.WebAPI.Configuration;
using Microsoft.Extensions.Options;

namespace Identity.WebAPI.Authentication;

sealed class ReturnUrlValidator(
    IOptions<IdpOptions> idpOptions,
    IOAuthClientRegistry clientRegistry
) : IReturnUrlValidator
{
    public bool IsValidAuthorizeReturnUrl(string returnUrl)
    {
        if (!Uri.TryCreate(returnUrl, UriKind.Absolute, out var uri))
            return false;

        var path = uri.AbsolutePath.TrimEnd('/');
        if (!path.EndsWith("/connect/authorize", StringComparison.OrdinalIgnoreCase))
            return false;

        return MatchesAuthority(uri);
    }

    public bool IsValidClientReturnUrl(string returnUrl)
    {
        if (!Uri.TryCreate(returnUrl, UriKind.Absolute, out var uri))
            return false;

        foreach (var client in clientRegistry.Clients)
        {
            if (string.IsNullOrWhiteSpace(client.Url))
                continue;

            if (!Uri.TryCreate(client.Url, UriKind.Absolute, out var clientUri))
                continue;

            if (UriMatchesOrigin(uri, clientUri))
                return true;
        }

        return false;
    }

    bool MatchesAuthority(Uri uri)
    {
        var authority = idpOptions.Value.Authority;
        if (string.IsNullOrWhiteSpace(authority))
            return uri.Host is "localhost" or "127.0.0.1";

        if (!Uri.TryCreate(authority, UriKind.Absolute, out var authorityUri))
            return false;

        return UriMatchesOrigin(uri, authorityUri);
    }

    static bool UriMatchesOrigin(Uri left, Uri right) =>
        string.Equals(left.Scheme, right.Scheme, StringComparison.OrdinalIgnoreCase)
        && string.Equals(left.Host, right.Host, StringComparison.OrdinalIgnoreCase)
        && left.Port == right.Port;
}
