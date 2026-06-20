using Identity.WebAPI.Exceptions;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace Identity.WebAPI.Controller;

[ApiController]
public abstract class IdentityControllerBase : ControllerBase
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
