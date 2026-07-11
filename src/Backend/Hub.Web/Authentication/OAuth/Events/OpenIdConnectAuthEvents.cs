using System.Security.Claims;
using Hub.Application.Abstractions;
using Hub.Web.Authentication.OAuth.Store;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;

namespace Hub.Web.Authentication.OAuth.Events;

sealed class OpenIdConnectAuthEvents(
    IOAuthTokenStore tokenStore,
    IUserProvisioningService userProvisioningService,
    IOptions<OAuthOptions> options,
    ILogger<OpenIdConnectAuthEvents> logger) : OpenIdConnectEvents
{
    readonly OAuthOptions _options = options.Value;

    public override Task RedirectToIdentityProvider(RedirectContext context)
    {
        context.ProtocolMessage.SetParameter(OAuthConstants.AudienceParameter, _options.Audience);
        return Task.CompletedTask;
    }

    public override Task AuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
    {
        context.TokenEndpointRequest?.SetParameter(OAuthConstants.AudienceParameter, _options.Audience);
        return Task.CompletedTask;
    }

    public override async Task TokenValidated(TokenValidatedContext context)
    {
        var response = context.TokenEndpointResponse;
        if (response?.AccessToken is null)
            return;

        var sessionId = Guid.NewGuid().ToString("N");
        var expiresIn = double.TryParse(response.ExpiresIn, out var seconds) ? seconds : 300;
        var expiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresIn);

        tokenStore.Store(
            sessionId,
            new OAuthTokens(response.AccessToken, response.RefreshToken, expiresAt));

        if (context.Principal?.Identity is ClaimsIdentity identity)
            identity.AddClaim(new Claim(OAuthConstants.Claims.SessionId, sessionId));

        var subject = context.Principal?.FindFirstValue(OAuthConstants.Claims.Subject);
        var nickname = context.Principal?.FindFirstValue(OAuthConstants.Claims.Username) ?? string.Empty;

        if (!Guid.TryParse(subject, out var userId))
            return;
        
        var result = await userProvisioningService.EnsureCreatedAsync(
            userId,
            new UserProvisioningParameters(nickname),
            context.HttpContext.RequestAborted);

        if (!result.IsSuccess)
        {
            logger.LogError(
                "User provisioning failed for {UserId}: {Errors}",
                userId,
                string.Join(", ", result.Errors));
        }
    }

    public override Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        logger.LogError(context.Exception, "OpenID Connect authentication failed.");
        return Task.CompletedTask;
    }
}
