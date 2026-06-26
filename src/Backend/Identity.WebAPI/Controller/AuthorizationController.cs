using System;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.WebAPI.Authentication.Audience;
using Identity.WebAPI.Authentication.Tokens;
using Identity.WebAPI.Configuration;
using Identity.WebAPI.Exceptions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Identity.WebAPI.Controller;

public sealed class AuthorizationController(
    UserManager<IdentityUser<Guid>> userManager,
    IOpenIddictApplicationManager applicationManager,
    ITokenClaimsBuilder tokenClaimsBuilder,
    IAudienceResolver audienceResolver,
    IOptions<IdpOptions> idpOptions
) : IdentityControllerBase
{
    [HttpGet("/connect/authorize")]
    public Task<IActionResult> AuthorizeGet() =>
        AuthorizeWithSessionAsync();

    [HttpPost("/connect/token"), Produces("application/json")]
    [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest();

        if (request is null)
            return InvalidOAuthRequest();

        if (request.IsAuthorizationCodeGrantType())
        {
            if (ResolveAudience(request) is not { } audience)
                return InvalidAudienceResponse();

            var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
            if (principal is null)
                return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            principal.SetAudiences(audience);
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsRefreshTokenGrantType())
        {
            if (ResolveAudience(request) is not { } audience)
                return InvalidAudienceResponse();

            var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

            var user = await userManager.FindByIdAsync(principal!.GetClaim(OpenIddictConstants.Claims.Subject)!);

            if (user is null || await userManager.IsLockedOutAsync(user))
                return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            var freshPrincipal = await tokenClaimsBuilder.BuildAsync(user, principal!.GetScopes());
            freshPrincipal.SetAudiences(audience);
            return SignIn(freshPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsClientCredentialsGrantType())
        {
            var application = await applicationManager.FindByClientIdAsync(request.ClientId ?? "");

            if (application is null)
                return BadRequest(new OpenIddictResponse
                {
                    Error = OpenIddictConstants.Errors.InvalidClient,
                    ErrorDescription = ApiErrors.OAuth.InvalidClient
                });

            if (ResolveAudience(request, request.ClientId) is not { } audience)
                return InvalidAudienceResponse();

            var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, OpenIddictConstants.Claims.Name, OpenIddictConstants.Claims.Role);

            identity.SetClaim(OpenIddictConstants.Claims.Subject, await applicationManager.GetClientIdAsync(application));
            identity.SetClaim(OpenIddictConstants.Claims.Name, await applicationManager.GetDisplayNameAsync(application));

            identity.SetDestinations(static claim => claim.Type switch
            {
                OpenIddictConstants.Claims.Name when claim.Subject!.HasScope(OpenIddictConstants.Permissions.Scopes.Profile)
                    => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],

                _ => [OpenIddictConstants.Destinations.AccessToken]
            });

            var principal = new ClaimsPrincipal(identity);
            principal.SetAudiences(audience);

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        return BadRequest(new OpenIddictResponse
        {
            Error = OpenIddictConstants.Errors.UnsupportedGrantType,
            ErrorDescription = ApiErrors.OAuth.UnsupportedGrantType
        });
    }

    async Task<IActionResult> AuthorizeWithSessionAsync()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request is null)
            return InvalidOAuthRequest();

        if (!IsAuthorizationCodeRequest(request))
            return UnsupportedResponseType();

        var user = await GetAuthenticatedUserFromCookieAsync();
        if (user is null)
            return RedirectToLoginPage();

        return await IssueAuthorizationCodeAsync(request, user);
    }

    async Task<IdentityUser<Guid>?> GetAuthenticatedUserFromCookieAsync()
    {
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
        if (!result.Succeeded || result.Principal is null)
            return null;

        return await userManager.GetUserAsync(result.Principal);
    }

    async Task<IActionResult> IssueAuthorizationCodeAsync(OpenIddictRequest request, IdentityUser<Guid> user)
    {
        if (await userManager.IsLockedOutAsync(user))
            return AccountLockedOut();

        var principal = await tokenClaimsBuilder.BuildAsync(user, request.GetScopes());
        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    RedirectResult RedirectToLoginPage()
    {
        var returnUrl = BuildAuthorizeReturnUrl();
        var loginPageUrl = idpOptions.Value.LoginPageUrl;

        if (string.IsNullOrWhiteSpace(loginPageUrl))
            throw new InvalidOperationException("IdP login page URL is not configured. Set Idp:LoginPageUrl.");

        return Redirect(QueryHelpers.AddQueryString(loginPageUrl, "returnUrl", returnUrl));
    }

    string BuildAuthorizeReturnUrl()
    {
        var request = HttpContext.Request;
        return $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}";
    }

    static bool IsAuthorizationCodeRequest(OpenIddictRequest request) =>
        request.IsAuthorizationCodeGrantType() || request.ResponseType == OpenIddictConstants.ResponseTypes.Code;

    string? ResolveAudience(OpenIddictRequest request, string? clientId = null) =>
        audienceResolver.ResolveFromTokenRequest(request, clientId ?? request.ClientId);

    static BadRequestObjectResult InvalidOAuthRequest() =>
        new(new OpenIddictResponse
        {
            Error = OpenIddictConstants.Errors.InvalidRequest,
            ErrorDescription = ApiErrors.OAuth.InvalidRequest
        });

    static BadRequestObjectResult UnsupportedResponseType() =>
        new(new OpenIddictResponse
        {
            Error = OpenIddictConstants.Errors.UnsupportedResponseType,
            ErrorDescription = "Only response_type=code"
        });

    static BadRequestObjectResult AccountLockedOut() =>
        new(new OpenIddictResponse
        {
            Error = OpenIddictConstants.Errors.AccessDenied,
            ErrorDescription = ApiErrors.OAuth.AccountLockedOut
        });

    static BadRequestObjectResult InvalidAudienceResponse() =>
        new(new OpenIddictResponse
        {
            Error = OpenIddictConstants.Errors.InvalidRequest,
            ErrorDescription = ApiErrors.OAuth.InvalidAudience
        });
}
