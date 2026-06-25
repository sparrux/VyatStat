using FluentResults;
using FluentResults.Extensions.AspNetCore;
using Identity.WebAPI.Authentication;
using Identity.WebAPI.Contracts;
using Identity.WebAPI.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace Identity.WebAPI.Controller;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public sealed class UsersController(
    IUsersService usersService,
    IAuthorizationService authorizationService
) : IdentityControllerBase
{
    const int MaxTakeUsers = 30;

    [HttpGet("/me")]
    public async Task<ActionResult<UserResponse>> GetMe()
    {
        var user = await usersService.GetUserAsync(UserId);
        return user.ToActionResult();
    }
    
    [HttpPut("/me/password")]
    public async Task<ActionResult> UpdatePassword(UpdatePasswordRequest request)
    {
        var result = await usersService.UpdatePasswordAsync(UserId, request);
        return result.ToActionResult();
    }
    
    [Authorize(Policy = Policies.ReadUsers, AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [HttpGet("/users/{userId:guid}")]
    public async Task<ActionResult<UserResponse>> GetUser(Guid userId)
    {
        var user = await usersService.GetUserAsync(userId);
        return user.ToActionResult();
    }
    
    [Authorize(Policy = Policies.ReadUsers, AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [HttpGet("/users")]
    public async Task<ActionResult<UsersResponse>> GetUsers(int take, int skip)
    {
        take = take > MaxTakeUsers ? MaxTakeUsers : take;
        take = take <= 0 ? 1 : take;
        skip = skip < 0 ? 0 : skip;
        
        var user = await usersService.GetUsersAsync(take, skip);
        return user.ToActionResult();
    }
    
    [HttpGet("/users/{userId:guid}/permissions")]
    public async Task<ActionResult<UserClaimsResponse>> GetUserPermissions(Guid userId)
    {
        Result<UserClaimsResponse>? result;

        var isOwner = UserId == userId;
        var canReadPermissions = (await CanReadUsersAsync()).Succeeded;

        if (isOwner || canReadPermissions)
        {
            result = await usersService.GetUserClaimsAsync(userId);
        }
        else
        {
            return Forbid();
        }
        
        return result.ToActionResult();
    }

    [Authorize(Policy = Policies.UpdateUserPermissions, AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [HttpPost("/users/{userId:guid}/permissions")]
    public async Task<ActionResult<UserClaimsResponse>> UpdateUserPermissions(
        Guid userId, UpdateUserPermissionsRequest request)
    {
        var result = await usersService.UpdateUserPermissionsAsync(userId, request);
        return result.ToActionResult();
    }
    
    [Authorize(Policy = Policies.LockOutUsers, AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [HttpPut("/users/{userId:guid}/lock")]
    public async Task<ActionResult> SetLockOutUser(
        Guid userId, bool lockout)
    {
        if (userId == UserId)
            return BadRequest("Cannot change lockout status for your own account");

        var result = await usersService.SetLockOutAsync(userId, lockout);
        return result.ToActionResult();
    }

    Task<AuthorizationResult> CanReadUsersAsync() =>
        authorizationService
            .AuthorizeAsync(User, Policies.ReadUsers);
}