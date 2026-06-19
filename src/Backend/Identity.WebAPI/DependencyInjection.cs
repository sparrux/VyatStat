using Identity.WebAPI.Authentication;
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
                policy.WithOrigins(clientUrl) // Адрес Angular приложения
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials(); // Нужно для обработки некоторых типов OAuth-запросов
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
        builder.Services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<ApplicationDbContext>();
            })
            .AddServer(options =>
            {
                // Enable the token endpoint.
                options.SetAuthorizationEndpointUris("connect/authorize")
                    .SetTokenEndpointUris("connect/token");
                
                // Enable the client credentials flow.
                options.AllowClientCredentialsFlow()
                    .AllowAuthorizationCodeFlow()
                    .AllowRefreshTokenFlow();
        
                options.RegisterScopes(OpenIddictConstants.Scopes.OpenId, OpenIddictConstants.Scopes.Profile);
        
                // Register the signing and encryption credentials.
                options.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

                // Register the ASP.NET Core host and configure the ASP.NET Core options.
                options.UseAspNetCore()
                    .EnableTokenEndpointPassthrough()
                    .EnableAuthorizationEndpointPassthrough();

                options.SetAccessTokenLifetime(TimeSpan.FromMinutes(5));
                options.SetRefreshTokenLifetime(TimeSpan.FromDays(30));
            })
            .AddValidation(options => 
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });
    }
}