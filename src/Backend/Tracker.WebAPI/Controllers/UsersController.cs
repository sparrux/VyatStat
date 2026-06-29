using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using Tracker.Application.Contracts.User.Responses;
using Tracker.Infrastructure.Persistence;

namespace Tracker.WebAPI.Controllers;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public sealed class UsersController(AppDbContext dbContext) : ApiControllerBase
{
    [HttpGet("/me")]
    public async Task<ActionResult<UserDetailsResponse>> GetMe(CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstAsync(u => u.Id == UserId, cancellationToken);

        return Ok(new UserDetailsResponse(
            user.Id,
            user.Nickname,
            [],
            user.CreatedAt));
    }
}
