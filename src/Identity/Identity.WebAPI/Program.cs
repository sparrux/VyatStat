using Identity.WebAPI.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using Scalar.AspNetCore;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("vyatka-identity"));
    options.UseOpenIddict();
});

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
    })
    .AddValidation(options => 
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

builder.Services.AddCors(options => {
    options.AddPolicy("ClientPolicy", policy => {
        policy.WithOrigins("http://localhost:4200") // Адрес Angular приложения
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Нужно для обработки некоторых типов OAuth-запросов
    });
});

var app = builder.Build();

// app.UseHttpsRedirection();

app.UseCors("ClientPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    await using var scope = app.Services.CreateAsyncScope();

    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();

    var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

    await SeedClientAsync(manager);
    
    // // Client Credentials
    // if (await manager.FindByClientIdAsync("service-worker") is null)
    // {
    //     await manager.CreateAsync(new()
    //     {
    //         ClientId = "service-worker",
    //         ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207",
    //         Permissions =
    //         {
    //             OpenIddictConstants.Permissions.Endpoints.Token,
    //             OpenIddictConstants.Permissions.GrantTypes.ClientCredentials
    //         }
    //     });
    // }
    //
    // // Authorization Code
    // if (await manager.FindByClientIdAsync("device") == null)
    // {
    //     await manager.CreateAsync(new()
    //     {
    //         ClientId = "device",
    //         ClientType = OpenIddictConstants.ClientTypes.Public,
    //         ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
    //         DisplayName = "Device client",
    //         Permissions =
    //         {
    //             OpenIddictConstants.Permissions.GrantTypes.DeviceCode,
    //             OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
    //             OpenIddictConstants.Permissions.Endpoints.DeviceAuthorization,
    //             OpenIddictConstants.Permissions.Endpoints.Token,
    //             OpenIddictConstants.Permissions.Scopes.Email,
    //             OpenIddictConstants.Permissions.Scopes.Profile,
    //             OpenIddictConstants.Permissions.Scopes.Roles,
    //         }
    //     });
    // }
}

app.Run();

// Функция регистрации вашего Angular в системе OAuth
static async Task SeedClientAsync(IOpenIddictApplicationManager manager) {
    if (await manager.FindByClientIdAsync("angular-client") is null) {
        await manager.CreateAsync(new()
        {
            ClientId = "angular-client",
            ClientType = OpenIddictConstants.ClientTypes.Public, // Публичный клиент (SPA)
            ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
            RedirectUris = { new("http://localhost:4200/callback") }, // Куда вернется Angular после логина
            Permissions = {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.Scopes.Profile,
                
                OpenIddictConstants.Scopes.OpenId,
                OpenIddictConstants.Scopes.Profile,
                OpenIddictConstants.Scopes.OfflineAccess
            }
        });
    }
}