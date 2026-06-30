using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using Tracker.Application.Services.Users;
using Tracker.Infrastructure.Persistence;
using Tracker.Infrastructure.Services.Users;

namespace Tracker.WebAPI;

static class DependencyInjection
{
    public static void AddWebServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        builder.Services.AddProblemDetails();

        builder.AddAuthentication();
        builder.AddCors();
        builder.AddEntityFrameworkCore();

        builder.Services.AddScoped<IUserProvisioningService, UserProvisioningService>();
    }

    static void AddAuthentication(this WebApplicationBuilder builder)
    {
        var authority = builder.Configuration["OpenIddict:Authority"];
        var audience = builder.Configuration["OpenIddict:Audience"];

        if (string.IsNullOrWhiteSpace(authority))
            throw new InvalidOperationException("OpenIddict:Authority is not configured.");

        if (string.IsNullOrWhiteSpace(audience))
            throw new InvalidOperationException("OpenIddict:Audience is not configured.");

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        });

        builder.Services.AddOpenIddict()
            .AddValidation(options =>
            {
                options.SetIssuer(authority);
                options.AddAudiences(audience);
                options.UseSystemNetHttp();
                options.UseAspNetCore();
            });

        builder.Services.AddAuthorization();
    }

    static void AddCors(this WebApplicationBuilder builder)
    {
        var origin = builder.Configuration["Clients:tracker-app:Url"];

        if (string.IsNullOrWhiteSpace(origin))
            throw new InvalidOperationException("Clients:tracker-app:Url is not configured for CORS.");

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(origin)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }

    static void AddEntityFrameworkCore(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("TrackerDb")));
    }
}
