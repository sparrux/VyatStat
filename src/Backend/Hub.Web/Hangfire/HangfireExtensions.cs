using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Hub.Application.Abstractions;

namespace Hub.Web.Hangfire;

static class HangfireExtensions
{
    const string DashboardPath = "/hangfire";
    const string SchemaName = "hangfire";

    extension(WebApplicationBuilder builder)
    {
        public void AddHangfire()
        {
            var connectionString = builder.Configuration.GetConnectionString("hubdb");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string 'hubdb' is not configured.");

            builder.Services.AddHangfire((_, config) =>
            {
                config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(
                        options => options.UseNpgsqlConnection(connectionString),
                        new PostgreSqlStorageOptions
                        {
                            SchemaName = SchemaName,
                            PrepareSchemaIfNecessary = true
                        });
            });

            builder.Services.AddHangfireServer();
            builder.Services.AddSingleton<IEventScheduler, HangfireEventScheduler>();
        }
    }

    extension(WebApplication app)
    {
        public void MapHangfireUi()
        {
            if (!app.Environment.IsDevelopment())
                return;

            app.MapHangfireDashboard(DashboardPath, new DashboardOptions
            {
                DashboardTitle = "Vyatka Hub",
                Authorization = [new LocalRequestsOnlyAuthorizationFilter()],
                DisplayStorageConnectionString = false
            });
        }
    }
}
