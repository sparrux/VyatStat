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

        var currentStamp = await cache.GetOrCreateAsync(SecurityStampCache.Key(userId), async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
            var user = await userManager.FindByIdAsync(userId);
            return user is null ? null : await userManager.GetSecurityStampAsync(user);
        });

        if (currentStamp is null || !string.Equals(currentStamp, tokenStamp, StringComparison.Ordinal))
        {
            await RejectStaleTokenAsync(context);
            return;
        }

        await next(context);
    }

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
