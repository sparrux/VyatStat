using System.Security.Cryptography.X509Certificates;
using Identity.WebAPI.Authentication;
using Identity.WebAPI.Authentication.Audience;
using Identity.WebAPI.Authentication.Tokens;
using Identity.WebAPI.Configuration;
using Identity.WebAPI.Exceptions;
using Identity.WebAPI.Persistence;
using Identity.WebAPI.Services.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using Scalar.AspNetCore;

namespace Identity.WebAPI;

static class DependencyInjection
{
    public static void AddWebServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<OpenIddictOptions>(
            builder.Configuration.GetSection(OpenIddictOptions.SectionName));

        builder.Services.Configure<IdpOptions>(options =>
        {
            builder.Configuration.GetSection(IdpOptions.SectionName).Bind(options);

            if (string.IsNullOrWhiteSpace(options.LoginPageUrl))
            {
                var webClientUrl = builder.Configuration["Clients:identity-app:Url"];
                if (!string.IsNullOrWhiteSpace(webClientUrl))
                    options.LoginPageUrl = $"{webClientUrl.TrimEnd('/')}/login";
            }
        });

        builder.Services.AddSingleton<IOAuthClientRegistry, OAuthClientRegistry>();
        builder.Services.AddSingleton<IAudienceResolver, AudienceResolver>();
        builder.Services.AddSingleton<IReturnUrlValidator, ReturnUrlValidator>();

        ValidateAudienceConfiguration(builder.Configuration);

        builder.Services.AddOpenApi();
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddControllers();
        builder.Services.AddMemoryCache();

        builder.Services.AddScoped<IUsersService, UsersService>();
        builder.Services.AddScoped<ITokenClaimsBuilder, TokenClaimsBuilder>();
        
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        });
        
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.Admin, policy => 
                policy.RequireClaim(UserClaimTypes.Role, UserClaims.Admin));
            
            options.AddPolicy(Policies.ReadUsers, policy => 
                policy.RequireClaim(UserClaimTypes.Permission, UserClaims.CanReadUsers));
            
            options.AddPolicy(Policies.UpdateUserPermissions, policy => 
                policy.RequireClaim(UserClaimTypes.Permission, UserClaims.CanUpdateUserPermissions));
            
            options.AddPolicy(Policies.LockOutUsers, policy => 
                policy.RequireClaim(UserClaimTypes.Permission, UserClaims.CanLockOutUsers));
        });
        
        builder.AddCors();
        builder.AddOpenIddict();
        builder.AddEntityFrameworkCore();
        builder.Services.AddConfiguredIdentity();
    }

    static void AddCors(this WebApplicationBuilder builder)
    {
        var origins = new OAuthClientRegistry(builder.Configuration).Clients
            .Select(client => client.Url)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Cast<string>()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (origins.Length == 0)
            throw new InvalidOperationException("OAuth client URLs are not configured for CORS.");

        builder.Services.AddCors(options => {
            options.AddDefaultPolicy(policy => {
                policy.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    static void AddEntityFrameworkCore(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("IdentityDb"));
            options.UseOpenIddict();
        });
    }
    
    static void AddConfiguredIdentity(this IServiceCollection services)
    {
        services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "Vyatka.IdP.Session";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(14);
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });
    }

    static void AddOpenIddict(this WebApplicationBuilder builder)
    {
        var audiences = builder.Configuration
            .GetSection(OpenIddictOptions.SectionName)
            .Get<OpenIddictOptions>()!
            .Audiences;

        builder.Services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<ApplicationDbContext>();
            })
            .AddServer(options =>
            {
                options.SetAuthorizationEndpointUris("connect/authorize")
                    .SetTokenEndpointUris("connect/token");
                
                options.AllowClientCredentialsFlow()
                    .AllowAuthorizationCodeFlow()
                    .AllowRefreshTokenFlow();
        
                options.RegisterScopes(
                    OpenIddictConstants.Scopes.OfflineAccess, 
                    OpenIddictConstants.Scopes.OpenId, 
                    OpenIddictConstants.Scopes.Profile
                );

                options.RegisterAudiences(audiences);

                if (builder.Environment.IsDevelopment())
                {
                    options.AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();
                }
                else
                {
                    var encryptionCertificate = X509CertificateLoader.LoadPkcs12FromFile(
                        "/app/certs/encryption.pfx", 
                        Environment.GetEnvironmentVariable("ENCRYPTION_CERT_PASSWORD"));

                    var signingCertificate = X509CertificateLoader.LoadPkcs12FromFile(
                        "/app/certs/signing.pfx", 
                        Environment.GetEnvironmentVariable("SIGNING_CERT_PASSWORD"));
                    
                    options.AddEncryptionCertificate(encryptionCertificate)
                        .AddSigningCertificate(signingCertificate);
                }

                options.DisableAccessTokenEncryption();
                
                options.UseAspNetCore()
                    .EnableTokenEndpointPassthrough()
                    .EnableAuthorizationEndpointPassthrough();

                options.SetAccessTokenLifetime(TimeSpan.FromMinutes(5));
                options.SetRefreshTokenLifetime(TimeSpan.FromDays(30));
            })
            .AddValidation(options => 
            {
                options.AddAudiences(audiences);
                options.UseLocalServer();
                options.UseAspNetCore();
            });
    }

    static void ValidateAudienceConfiguration(IConfiguration configuration)
    {
        var openIddictOptions = configuration
            .GetSection(OpenIddictOptions.SectionName)
            .Get<OpenIddictOptions>();

        if (openIddictOptions?.Audiences is not { Length: > 0 } audiences)
            throw new InvalidOperationException(
                "OpenIddict audiences are not configured. Add at least one audience to OpenIddict:Audiences");

        var allowedAudiences = audiences.ToHashSet(StringComparer.Ordinal);
        var registry = new OAuthClientRegistry(configuration);

        foreach (var client in registry.Clients)
        {
            if (string.IsNullOrWhiteSpace(client.Audience))
                throw new InvalidOperationException(
                    $"Client '{client.ClientId}' is missing Clients:*:Audience configuration");

            if (!allowedAudiences.Contains(client.Audience))
                throw new InvalidOperationException(
                    $"Client '{client.ClientId}' uses audience '{client.Audience}' that is not listed in OpenIddict:Audiences");
        }
    }

    public static void MapApiDocs(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return;

        app.MapOpenApi();
        app.MapScalarApiReference();
    }
}