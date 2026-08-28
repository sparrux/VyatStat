using Hub.Web.Authentication.OAuth;
using Hub.Web.Authentication.OAuth.Events;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Hub.Web.Authentication;

static class AuthenticationExtensions
{
    extension (WebApplicationBuilder builder)
    {
        public void AddAuthentication()
        {
            var oauthSection = builder.Configuration.GetSection(OAuthOptions.SectionName);
            var oauthOptions = oauthSection.Get<OAuthOptions>()!;
            
            oauthOptions.EnsureValid();
            
            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = OAuthConstants.SmartAuthScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddPolicyScheme(
                    OAuthConstants.SmartAuthScheme,
                    "Cookie or Bearer",
                    options =>
                    {
                        options.ForwardDefaultSelector = context =>
                        {
                            var authorization = context.Request.Headers.Authorization.ToString();
                            return authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                                ? JwtBearerDefaults.AuthenticationScheme
                                : CookieAuthenticationDefaults.AuthenticationScheme;
                        };
                    })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = OAuthConstants.CookieName;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
                        ? CookieSecurePolicy.SameAsRequest
                        : CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromDays(30);
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
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = oauthOptions.Authority;
                    options.ClientId = oauthOptions.ClientId;
                    options.CallbackPath = oauthOptions.CallbackPath;
                    options.ResponseType = "code";
                    options.UsePkce = true;
                    options.SaveTokens = false;
                    options.GetClaimsFromUserInfoEndpoint = false;
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                    options.MapInboundClaims = false;

                    options.Scope.Clear();
                    foreach (var scope in oauthOptions.Scopes.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                        options.Scope.Add(scope);

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = OAuthConstants.Claims.Username,
                        RoleClaimType = "role"
                    };

                    options.EventsType = typeof(OpenIdConnectAuthEvents);
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = oauthOptions.Authority;
                    options.Audience = oauthOptions.Audience;
                    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidAudience = oauthOptions.Audience,
                        NameClaimType = OAuthConstants.Claims.Username,
                        RoleClaimType = "role"
                    };
                });
        }
    }
}