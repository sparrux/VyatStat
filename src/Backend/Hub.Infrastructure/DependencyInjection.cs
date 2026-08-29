using Hangfire;
using Hangfire.PostgreSql;
using Hub.Application.Abstractions;
using Hub.Infrastructure.Hangfire;
using Hub.Infrastructure.Persistence;
using Hub.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hub.Infrastructure;

public static class DependencyInjection
{
    const string HangfireSchema = "hangfire";

    public static void AddInfrastructure(
        this IServiceCollection services,
        string dbConnectionName,
        IConfiguration configuration)
    {
        services.TryAddSingleton(TimeProvider.System);

        services.AddSingleton<AuditInterceptor>();
        services.AddDbContext<HubDbContext>((provider, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString(dbConnectionName));

            options.AddInterceptors(provider.GetRequiredService<AuditInterceptor>());
        });
        services.AddScoped<IHubDbContext>(sp => sp.GetRequiredService<HubDbContext>());

        services.AddHangfire(dbConnectionName, configuration);
    }

    static void AddHangfire(this IServiceCollection services, string dbConnectionName, IConfiguration configuration)
    {
        services.AddHangfireStorage(dbConnectionName, configuration);
        
        services.AddSingleton<IEventScheduler, HangfireEventScheduler>();
        services.AddScoped<IEventStateJobs, EventStateTransitionProcessor>();
    }

    static void AddHangfireStorage(this IServiceCollection services, string dbConnectionName, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(dbConnectionName);
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException($"Connection string '{dbConnectionName}' is not configured.");

        services.AddHangfire((_, config) =>
        {
            config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(
                    options => options.UseNpgsqlConnection(connectionString),
                    new PostgreSqlStorageOptions
                    {
                        SchemaName = HangfireSchema,
                        PrepareSchemaIfNecessary = true
                    });
        });
    }
}
