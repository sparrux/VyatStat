using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using OpenIddict.Abstractions;

namespace Identity.WebAPI.Authentication;

sealed class SecurityStampValidationMiddleware(
    RequestDelegate next,
    IMemoryCache cache
)
{
    public async Task InvokeAsync(HttpContext context, UserManager<IdentityUser<Guid>> userManager)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await next(context);
            return;
        }

        var userId = context.User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        var tokenStamp = context.User.FindFirst(UserClaimTypes.SecurityStamp)?.Value;

        if (userId is null)
        {
            await next(context);
            return;
        }

        if (tokenStamp is null)
        {
            await RejectStaleTokenAsync(context);
            return;
        }

        var authState = await cache.GetOrCreateAsync(SecurityStampCache.Key(userId), async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return new UserAuthState(null, true);

            return new UserAuthState(
                await userManager.GetSecurityStampAsync(user),
                await userManager.IsLockedOutAsync(user));
        });

        if (authState is null
            || authState.Stamp is null
            || !string.Equals(authState.Stamp, tokenStamp, StringComparison.Ordinal)
            || authState.IsLockedOut)
        {
            await RejectStaleTokenAsync(context);
            return;
        }

        await next(context);
    }

    sealed record UserAuthState(string? Stamp, bool IsLockedOut);

    static Task RejectStaleTokenAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.Headers.Append("X-Token-Stale", "1");
        return Task.CompletedTask;
    }
}

static class SecurityStampValidationMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityStampValidation(this IApplicationBuilder app) =>
        app.UseMiddleware<SecurityStampValidationMiddleware>();
}
