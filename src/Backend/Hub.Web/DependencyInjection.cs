using Asp.Versioning;
using Hub.Application.Abstractions;
using Hub.Web.Auth;
using Hub.Web.Endpoints;
using Hub.Web.OpenApi;
using Hub.Web.Services.Users;
using OpenIddict.Validation.AspNetCore;
using ServiceDefaults;

namespace Hub.Web;

static class DependencyInjection
{
    extension(WebApplicationBuilder builder)
    {
        public void AddWeb()
        {
            builder.AddServiceDefaults();
            
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IUserContext, CurrentUserContext>();
            builder.Services.AddScoped<IUserProvisioningService, UserProvisioningService>();
            
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;

                options.ApiVersionReader =
                    new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<OAuth2SecuritySchemeTransformer>();
            });

            builder.AddAuthentication();
            builder.AddCors();
            builder.Services.AddAuthorization();
        }

        void AddCors()
        {
            var origin = builder.Configuration["Clients:hub-app:Url"];

            if (string.IsNullOrWhiteSpace(origin))
                throw new InvalidOperationException("Clients:hub-app:Url is not configured for CORS.");

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
        
        void AddAuthentication()
        {
            var authority = builder.Configuration["OpenIddict:Authority"];
            var audience = builder.Configuration["OpenIddict:Audience"];

            if (string.IsNullOrWhiteSpace(authority))
                throw new InvalidOperationException("OpenIddict:Authority is not configured.");

            if (string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("OpenIddict:Audience is not configured.");

            builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

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
    }

    extension(WebApplication app)
    {
        public void MapEndpoints()
        {
            app.MapEventEndpoints();
        }

        public void MapWebOpenApi()
        {
            app.MapOpenApi();
            app.MapScalar();
        }
    }
}