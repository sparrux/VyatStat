using Hub.Application.Abstractions;
using OpenIddict.Abstractions;

namespace Hub.Web.Authentication;

sealed class UserProvisioningMiddleware(RequestDelegate next, ILogger<UserProvisioningMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IUserProvisioningService userProvisioningService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var subject = context.User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
            var nickname = context.User.FindFirst(OpenIddictConstants.Claims.Username)?.Value;

            if (Guid.TryParse(subject, out var userId) && !string.IsNullOrWhiteSpace(nickname))
            {
                var result = await userProvisioningService.EnsureCreatedAsync(
                    userId,
                    new UserProvisioningParameters(nickname),
                    context.RequestAborted);

                if (!result.IsSuccess)
                {
                    logger.LogError(
                        "Cannot ensure user creation for {UserId}: {Errors}",
                        userId,
                        string.Join(", ", result.Errors));
                }
            }
        }

        await next(context);
    }
}

static class UserProvisioningMiddlewareExtensions
{
    public static IApplicationBuilder UseUserProvisioning(this IApplicationBuilder app) =>
        app.UseMiddleware<UserProvisioningMiddleware>();
}
