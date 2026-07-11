using System.Security.Claims;
using Hub.Application.Abstractions;
using Hub.Web.Authentication.OAuth;

namespace Hub.Web.Authentication;

sealed class CurrentUserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid UserId =>
        Guid.Parse(
            httpContextAccessor
                .HttpContext!
                .User
                .FindFirstValue(OAuthConstants.Claims.Subject)
                ?? httpContextAccessor
                    .HttpContext!
                    .User
                    .FindFirstValue(ClaimTypes.NameIdentifier)!);
}
