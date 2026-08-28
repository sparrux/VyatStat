using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Hub.Web.Authentication.OAuth.Store;

sealed class MemoryOAuthTokenStore(
    IMemoryCache cache, // TODO: DistributedCache
    IOptions<OAuthOptions> options
) : IOAuthTokenStore
{
    const string KeyPrefix = "oauth:tokens:";

    public void Store(string sessionId, OAuthTokens tokens) =>
        cache.Set(KeyPrefix + sessionId, tokens, options.Value.TokenLifetime);

    public OAuthTokens? Get(string sessionId)
    {
        cache.TryGetValue(KeyPrefix + sessionId, out OAuthTokens? tokens);
        return tokens;
    }

    public void Remove(string sessionId) =>
        cache.Remove(KeyPrefix + sessionId);
}
