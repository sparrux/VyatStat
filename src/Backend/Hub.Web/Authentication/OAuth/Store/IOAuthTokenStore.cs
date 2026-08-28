namespace Hub.Web.Authentication.OAuth.Store;

interface IOAuthTokenStore
{
    void Store(string sessionId, OAuthTokens tokens);

    OAuthTokens? Get(string sessionId);

    void Remove(string sessionId);
}
