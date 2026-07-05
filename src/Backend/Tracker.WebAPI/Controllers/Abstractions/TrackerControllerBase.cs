using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace Tracker.WebAPI.Controllers.Abstractions;

[ApiController]
public abstract class TrackerControllerBase : ControllerBase
{
    protected Guid UserId
    {
        get
        {
            var userId = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException();

            if (!Guid.TryParse(userId, out var parsedUserId))
                throw new FormatException("Invalid user identifier");

            return parsedUserId;
        }
    }
}
