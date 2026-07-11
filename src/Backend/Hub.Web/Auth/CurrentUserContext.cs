using Hub.Application.Abstractions;
using OpenIddict.Abstractions;

namespace Hub.Web.Auth;

sealed class CurrentUserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid UserId =>
        Guid.Parse(
            httpContextAccessor
                .HttpContext!
                .User
                .FindFirst(OpenIddictConstants.Claims.Subject)!
                .Value);
}