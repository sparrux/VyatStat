using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using Tracker.WebAPI.Exceptions;

namespace Tracker.WebAPI.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected Guid UserId
    {
        get
        {
            var userId = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException();

            if (!Guid.TryParse(userId, out var parsedUserId))
                throw new FormatException(ApiErrors.InvalidUserIdentifier);

            return parsedUserId;
        }
    }
}
