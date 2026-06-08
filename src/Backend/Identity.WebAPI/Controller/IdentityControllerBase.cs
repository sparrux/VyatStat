using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Identity.WebAPI.Controller;

[ApiController]
public abstract class IdentityControllerBase : ControllerBase
{
    protected Guid UserId
    {
        get
        {
            if (User.FindFirst("sub")?.Value is var userId && string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("User identifier is required");
        
            return Guid.Parse(userId);
        }
    }
}