using Identity.WebAPI.Authentication;
using Identity.WebAPI.Configuration;
using Identity.WebAPI.Persistence;
using Identity.WebAPI.Services.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;

namespace Identity.WebAPI;

static class DependencyInjection
{
    public static void AddWebServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<OpenIddictOptions>(
            builder.Configuration.GetSection(OpenIddictOptions.SectionName));

        builder.Services.AddSingleton<IOAuthClientRegistry, OAuthClientRegistry>();
        builder.Services.AddSingleton<IAudienceResolver, AudienceResolver>();

        ValidateAudienceConfiguration(builder.Configuration);

        builder.Services.AddOpenApi();
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
        var clientUrl = builder.Configuration.GetSection("Clients:WebClient:Url").Value;

        if (string.IsNullOrWhiteSpace(clientUrl))
            throw new InvalidOperationException("Web client CORS not configured. Web client URL is missed in config");
        
        builder.Services.AddCors(options => {
            options.AddDefaultPolicy(policy => {
                policy.WithOrigins(clientUrl)
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
            options.UseNpgsql(builder.Configuration.GetConnectionString("vyatka-identity"));
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
        
                // Register the signing and encryption credentials.
                options.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

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
}