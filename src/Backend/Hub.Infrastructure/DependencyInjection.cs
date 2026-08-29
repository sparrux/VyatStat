using Hub.Application.Abstractions;
using Hub.Infrastructure.Persistence;
using Hub.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hub.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<AuditInterceptor>();
        services.AddDbContext<HubDbContext>((provider, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("hubdb"));
            
            options.AddInterceptors(provider.GetRequiredService<AuditInterceptor>());
        });
        services.AddScoped<IHubDbContext>(sp => sp.GetRequiredService<HubDbContext>());
    }
}