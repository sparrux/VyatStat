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
            if (User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value is var userId && string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("User identifier is required");
        
            return Guid.Parse(userId);
        }
    }
}