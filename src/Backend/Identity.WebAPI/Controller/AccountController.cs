using FluentResults;
using FluentResults.Extensions.AspNetCore;
using Identity.WebAPI.Authentication;
using Identity.WebAPI.Contracts;
using Identity.WebAPI.Services.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace Identity.WebAPI.Controller;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class AccountController(
    IAccountService accountService,
    IAuthorizationService authorizationService
) : IdentityControllerBase
{
    [AllowAnonymous]
    [HttpPost("/register")]
    public async Task<IActionResult> Register(RegistrationRequest request)
    {
        var result = await accountService.CreateAsync(request);
        return result.ToActionResult();
    }
    
    [HttpGet("/profile")]
    public async Task<ActionResult<ProfileResponse>> GetProfileInfo()
    {
        var profile = await accountService.GetProfileAsync(UserId);
        return profile.ToActionResult();
    }
    
    [HttpGet("/{userId:guid}/permissions")]
    public async Task<ActionResult<UserClaimsResponse>> GetUserPermissions(Guid userId)
    {
        Result<UserClaimsResponse>? result;

        var isOwner = UserId == userId;
        var canReadPermissions = (await CanReadUsersAsync()).Succeeded;

        if (isOwner || canReadPermissions)
        {
            result = await accountService.GetUserClaimsAsync(userId);
        }
        else
        {
            result = Result.Fail<UserClaimsResponse>("Has no access");
        }
            
        return result.ToActionResult();
    }

    [Authorize(Policy = Policies.UpdateUserPermissions, AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [HttpPost("/{userId:guid}/permissions")]
    public async Task<ActionResult<UserClaimsResponse>> UpdateUserPermissions(
        Guid userId, UpdateUserPermissionsRequest request)
    {
        var result = await accountService.UpdateUserPermissionsAsync(UserId, request);
        return result.ToActionResult();
    }

    Task<AuthorizationResult> CanReadUsersAsync() =>
        authorizationService
            .AuthorizeAsync(User, Policies.ReadUsers);
}