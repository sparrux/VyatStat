using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using Scalar.AspNetCore;
using Tracker.Application.Services.Users;
using Tracker.Infrastructure.Persistence;
using Tracker.Infrastructure.Services.Users;
using Tracker.WebAPI.OpenApi;

namespace Tracker.WebAPI;

static class DependencyInjection
{
    public static void AddWebServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<OAuth2SecuritySchemeTransformer>();
        });
        builder.Services.AddControllers();
        builder.Services.AddFluentValidationAutoValidation();
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
    
    public static void MapApiDocs(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference((options, httpContext) =>
        {
            var authority = app.Configuration["OpenIddict:Authority"]?.TrimEnd('/');
            var audience = app.Configuration["OpenIddict:Audience"];
            var clientId = app.Configuration["Clients:tracker-scalar:ClientId"];

            if (string.IsNullOrWhiteSpace(authority))
                throw new InvalidOperationException("OpenIddict:Authority is not configured.");

            if (string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("OpenIddict:Audience is not configured.");

            if (string.IsNullOrWhiteSpace(clientId))
                throw new InvalidOperationException("Clients:tracker-scalar:ClientId is not configured.");

            var scalarBaseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/scalar/v1";

            options
                .AddPreferredSecuritySchemes("oauth2")
                .AddAuthorizationCodeFlow("oauth2", flow =>
                {
                    flow.ClientId = clientId;
                    flow.Pkce = Pkce.Sha256;
                    flow.RedirectUri = scalarBaseUrl;
                    flow.SelectedScopes =
                    [
                        OpenIddictConstants.Scopes.OpenId,
                        OpenIddictConstants.Scopes.Profile,
                        OpenIddictConstants.Scopes.OfflineAccess
                    ];

                    flow.WithCredentialsLocation(CredentialsLocation.Body);
                    flow.AddBodyParameter(OpenIddictConstants.Parameters.ClientId, clientId);
                    flow.AddBodyParameter(OpenIddictConstants.Claims.Audience, audience);
                    flow.AddQueryParameter(OpenIddictConstants.Parameters.RedirectUri, scalarBaseUrl);
                    flow.AddQueryParameter(OpenIddictConstants.Claims.Audience, audience);
                });
        });
    }
}
