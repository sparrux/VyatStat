using Hub.Application.Abstractions;
using Hub.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hub.Infrastructure.Persistence.Extensions;

static class PersistenceDependencyExtensions
{
    extension(IServiceCollection services)
    {
        public void AddPersistenceServices(string dbConnectionName, IConfiguration configuration)
        {
            services.AddSingleton<AuditInterceptor>();
            services.AddDbContext<HubDbContext>((provider, options) =>
            {
                options.UseNpgsql(configuration.GetConnectionString(dbConnectionName));

                options.AddInterceptors(provider.GetRequiredService<AuditInterceptor>());
            });
            services.AddScoped<IHubDbContext>(sp => sp.GetRequiredService<HubDbContext>());
        }
    }
}