using System.Security.Claims;
using Identity.WebAPI.Authentication;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Identity.WebAPI.Controller;

public class AuthorizationController(
    UserManager<IdentityUser<Guid>> userManager,
    SignInManager<IdentityUser<Guid>> signInManager,
    IOpenIddictApplicationManager applicationManager,
    ITokenClaimsBuilder tokenClaimsBuilder
) : IdentityControllerBase
{
    [HttpGet("/connect/authorize")]
    [HttpPost("/connect/authorize")]
    [Consumes("application/x-www-form-urlencoded")] // OAuth 2.0 стандарт требования к контенту
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ?? 
            throw new InvalidOperationException("Запрос не является валидным OAuth 2.0 запросом.");
        
        if (!request.IsAuthorizationCodeGrantType() && request.ResponseType != OpenIddictConstants.ResponseTypes.Code)
        {
            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.UnsupportedResponseType,
                ErrorDescription = "Разрешен только response_type=code."
            });
        }

        // 1. Поиск пользователя (в OAuth поле для логина называется Username)
        var user = await userManager.FindByNameAsync(request.Username!);
        if (user == null)
        {
            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.InvalidGrant,
                ErrorDescription = "Неверный логин или пароль."
            });
        }

        // 2. Проверка пароля без создания куки (CheckPasswordSignInAsync)
        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password!, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.InvalidGrant,
                ErrorDescription = "Неверный логин или пароль."
            });
        }
        
        var principal = await tokenClaimsBuilder.BuildAsync(user, request.GetScopes());

        // OpenIddict перехватит этот SignIn и сделает редирект обратно в Angular (на /callback) с параметром ?code=...
        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
    
    [HttpPost("/connect/token"), Produces("application/json")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException("Некорректный OAuth-запрос.");

        // Сценарий А: Обмен временного Authorization Code на токены (с проверкой PKCE)
        if (request.IsAuthorizationCodeGrantType())
        {
            // Извлекаем principal, который мы сохранили на этапе эндпоинта авторизации
            var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
            
            return SignIn(principal!, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        // Сценарий Б: Обновление токена по Refresh Token
        if (request.IsRefreshTokenGrantType())
        {
            var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
            
            var user = await userManager.FindByIdAsync(principal!.GetClaim(OpenIddictConstants.Claims.Subject)!);
            if (user is null || await userManager.IsLockedOutAsync(user))
                return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            var freshPrincipal = await tokenClaimsBuilder.BuildAsync(user, principal.GetScopes());
            return SignIn(freshPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        
        if (request.IsClientCredentialsGrantType())
        {
            // Note: the client credentials are automatically validated by OpenIddict:
            // if client_id or client_secret are invalid, this action won't be invoked.

            var application = await applicationManager.FindByClientIdAsync(request.ClientId) ??
                              throw new InvalidOperationException("The application cannot be found.");

            // Create a new ClaimsIdentity containing the claims that
            // will be used to create an id_token, a token or a code.
            var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, OpenIddictConstants.Claims.Name, OpenIddictConstants.Claims.Role);

            // Use the client_id as the subject identifier.
            identity.SetClaim(OpenIddictConstants.Claims.Subject, await applicationManager.GetClientIdAsync(application));
            identity.SetClaim(OpenIddictConstants.Claims.Name, await applicationManager.GetDisplayNameAsync(application));

            identity.SetDestinations(static claim => claim.Type switch
            {
                // Allow the "name" claim to be stored in both the access and identity tokens
                // when the "profile" scope was granted (by calling principal.SetScopes(...)).
                OpenIddictConstants.Claims.Name when claim.Subject.HasScope(OpenIddictConstants.Permissions.Scopes.Profile)
                    => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],

                // Otherwise, only store the claim in the access tokens.
                _ => [OpenIddictConstants.Destinations.AccessToken]
            });

            return SignIn(new(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        return BadRequest(new OpenIddictResponse
        {
            Error = OpenIddictConstants.Errors.UnsupportedGrantType,
            ErrorDescription = "Данный тип гранта не поддерживается."
        });
    }
}