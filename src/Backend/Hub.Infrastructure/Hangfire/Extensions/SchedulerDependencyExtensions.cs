using Hangfire;
using Hangfire.PostgreSql;
using Hub.Application.Abstractions;
using Hub.Infrastructure.Hangfire.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hub.Infrastructure.Hangfire.Extensions;

static class SchedulerDependencyExtensions
{
    const string HangfireSchema = "hangfire";
    
    extension(IServiceCollection services)
    {
        public void AddSchedulerServices(string dbConnectionName, IConfiguration configuration)
        {
            services.AddSingleton<IEventScheduler, HangfireEventScheduler>();
            services.AddScoped<IEventStateJobs, EventStateTransitionProcessor>();
            
            services.AddHangfireStorage(dbConnectionName, configuration);
        }

        private void AddHangfireStorage(string dbConnectionName, IConfiguration configuration)
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
}