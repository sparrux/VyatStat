using System.Net.Mime;
using System.Security.Claims;
using Identity.WebAPI.Authentication;
using Identity.WebAPI.Exceptions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Identity.WebAPI.Controller;

public sealed class AuthorizationController(
    UserManager<IdentityUser<Guid>> userManager,
    SignInManager<IdentityUser<Guid>> signInManager,
    IOpenIddictApplicationManager applicationManager,
    ITokenClaimsBuilder tokenClaimsBuilder,
    IAudienceResolver audienceResolver
) : IdentityControllerBase
{
    [HttpPost("/connect/authorize")]
    [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        
        if (request is null)
            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.InvalidRequest,
                ErrorDescription = ApiErrors.OAuth.InvalidRequest
            });
        
        if (!request.IsAuthorizationCodeGrantType() && request.ResponseType != OpenIddictConstants.ResponseTypes.Code)
            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.UnsupportedResponseType,
                ErrorDescription = "Only response_type=code"
            });

        var user = await userManager.FindByNameAsync(request.Username!);
        if (user is null)
            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.InvalidGrant,
                ErrorDescription = ApiErrors.OAuth.InvalidUserCredentials
            });

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password!, lockoutOnFailure: false);
        if (!result.Succeeded)
            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.InvalidGrant,
                ErrorDescription = ApiErrors.OAuth.InvalidUserCredentials
            });

        if (await userManager.IsLockedOutAsync(user))
            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.AccessDenied,
                ErrorDescription = ApiErrors.OAuth.AccountLockedOut
            });
        
        var principal = await tokenClaimsBuilder.BuildAsync(user, request.GetScopes());

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
    
    [HttpPost("/connect/token"), Produces("application/json")]
    [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        
        if (request is null)
            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.InvalidRequest,
                ErrorDescription = ApiErrors.OAuth.InvalidRequest
            });

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

    string? ResolveAudience(OpenIddictRequest request, string? clientId = null) =>
        audienceResolver.ResolveFromTokenRequest(request, clientId ?? request.ClientId);

    static BadRequestObjectResult InvalidAudienceResponse() =>
        new(new OpenIddictResponse
        {
            Error = OpenIddictConstants.Errors.InvalidRequest,
            ErrorDescription = ApiErrors.OAuth.InvalidAudience
        });
}
