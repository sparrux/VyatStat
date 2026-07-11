namespace Hub.Web.Authentication.OAuth;

sealed record OAuthTokens(
    string AccessToken,
    string? RefreshToken,
    DateTimeOffset AccessTokenExpiresAt);
