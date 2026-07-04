using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using Tracker.Application.Contracts.Common.Requests;
using Tracker.Application.Contracts.Users.Requests;
using Tracker.Application.Contracts.Users.Responses;
using Tracker.Application.Services.Users;

namespace Tracker.WebAPI.Controllers;

[Route("users")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public sealed class UsersController(IUsersService usersService) : ApiControllerBase
{
    [HttpGet("me")]
    public Task<ActionResult<UserDetailsResponse>> GetMe()
    {
        return GetUser(UserId);
    }
    
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<UserDetailsResponse>> GetUser(Guid userId)
    {
        var result = await usersService.GetDetailsAsync(userId);
        return result.ToActionResult();
    }
    
    [HttpPut("{userId:guid}/info")]
    public async Task<ActionResult<UserDetailsResponse>> UpdateUser(UpdateUserRequest request)
    {
        var result = await usersService.UpdateAsync(UserId, request);
        return result.ToActionResult();
    }
    
    [HttpGet]
    public async Task<ActionResult<UsersListResponse>> GetUsers([FromQuery] PageSelectionRequest request)
    {
        var result = await usersService.GetListAsync(request.Offset, request.Take);
        return result.ToActionResult();
    }
}
