using OpenIddict.Abstractions;
using Tracker.Application.Interfaces.Users;

namespace Tracker.WebAPI.Authentication;

sealed class UserProvisioningMiddleware(RequestDelegate next, ILogger<UserProvisioningMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IUserProvisioningService userProvisioningService)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await next(context);
            return;
        }

        var subject = context.User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        var nickname = context.User.FindFirst(OpenIddictConstants.Claims.Username)?.Value;

        if (string.IsNullOrWhiteSpace(subject) || !Guid.TryParse(subject, out var userId))
        {
            await next(context);
            return;
        }

        var userCreation = await userProvisioningService.EnsureCreatedAsync(
            userId, 
            new UserCreationParameters(nickname!),
            context.RequestAborted);

        if (userCreation.IsFailed)
        {
            logger.LogError("Cannot ensure user creation: {Error}", userCreation.Errors.First().Message);
        }
        
        await next(context);
    }
}

static class UserProvisioningMiddlewareExtensions
{
    public static IApplicationBuilder UseUserProvisioning(this IApplicationBuilder app) =>
        app.UseMiddleware<UserProvisioningMiddleware>();
}
