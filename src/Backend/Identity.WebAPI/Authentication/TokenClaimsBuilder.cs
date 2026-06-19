using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;

namespace Identity.WebAPI.Authentication;

sealed class TokenClaimsBuilder(UserManager<IdentityUser<Guid>> userManager) : ITokenClaimsBuilder
{
    public async Task<ClaimsPrincipal> BuildAsync(IdentityUser<Guid> user, IEnumerable<string>? scopes = null)
    {
        var claims = await userManager.GetClaimsAsync(user);
        var securityStamp = await userManager.GetSecurityStampAsync(user);

        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role);

        identity.AddClaim(OpenIddictConstants.Claims.Subject, user.Id.ToString());
        identity.AddClaim(OpenIddictConstants.Claims.Username, user.UserName!);
        identity.AddClaims(claims);

        if (!string.IsNullOrEmpty(securityStamp))
            identity.AddClaim(UserClaimTypes.SecurityStamp, securityStamp);

        identity.SetDestinations(_ =>
        [
            OpenIddictConstants.Destinations.AccessToken,
            OpenIddictConstants.Destinations.IdentityToken
        ]);

        var principal = new ClaimsPrincipal(identity);

        if (scopes is not null)
            principal.SetScopes(scopes);

        return principal;
    }
}
