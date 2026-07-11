using Hub.Application.Features.Common.Contracts;
using Hub.Web.Authentication.OAuth;
using Hub.Web.Authentication.OAuth.Store;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Hub.Web.Endpoints;

static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var auth = app.MapGroup("/auth");

        auth.MapGet("/login", Login);
        auth.MapPost("/logout", Logout);
        auth.MapGet("/session", Session).RequireAuthorization();
    }

    static IResult Login(
        [FromQuery] string? returnUrl,
        IOptions<OAuthOptions> options,
        IConfiguration configuration)
    {
        var resolvedReturnUrl = ResolveReturnUrl(returnUrl, options.Value, configuration);

        return Results.Challenge(
            new AuthenticationProperties { RedirectUri = resolvedReturnUrl },
            [OpenIdConnectDefaults.AuthenticationScheme]);
    }

    static async Task<IResult> Logout(
        HttpContext httpContext,
        IOAuthTokenStore tokenStore)
    {
        var sessionId = httpContext.User.FindFirst(OAuthConstants.Claims.SessionId)?.Value;
        if (!string.IsNullOrWhiteSpace(sessionId))
            tokenStore.Remove(sessionId);

        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Results.NoContent();
    }

    static IResult Session(HttpContext httpContext)
    {
        if (httpContext.User.Identity?.IsAuthenticated != true)
            return Results.Unauthorized();

        var subject = httpContext.User.FindFirst(OAuthConstants.Claims.Subject)?.Value;
        var username = httpContext.User.FindFirst(OAuthConstants.Claims.Username)?.Value;

        if (!Guid.TryParse(subject, out var userId))
            return Results.Unauthorized();

        return Results.Ok(new UserSummaryResponse(userId, username ?? string.Empty));
    }

    static string ResolveReturnUrl(string? returnUrl, OAuthOptions options, IConfiguration configuration)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && IsAllowedReturnUrl(returnUrl, configuration))
            return returnUrl;

        var hubAppUrl = configuration["Clients:hub-app:Url"]?.TrimEnd('/');
        if (!string.IsNullOrWhiteSpace(hubAppUrl))
            return hubAppUrl;

        return options.DefaultReturnUrl;
    }

    static bool IsAllowedReturnUrl(string returnUrl, IConfiguration configuration)
    {
        if (!Uri.TryCreate(returnUrl, UriKind.Absolute, out var uri))
            return false;

        var hubAppUrl = configuration["Clients:hub-app:Url"];
        if (string.IsNullOrWhiteSpace(hubAppUrl))
            return false;

        if (!Uri.TryCreate(hubAppUrl, UriKind.Absolute, out var allowedOrigin))
            return false;

        return uri.Scheme == allowedOrigin.Scheme
            && uri.Host.Equals(allowedOrigin.Host, StringComparison.OrdinalIgnoreCase)
            && uri.Port == allowedOrigin.Port;
    }
}
