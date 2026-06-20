using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Identity.WebAPI.Authentication.Tokens;

public interface ITokenClaimsBuilder
{
    Task<ClaimsPrincipal> BuildAsync(IdentityUser<Guid> user, IEnumerable<string>? scopes = null);
}
