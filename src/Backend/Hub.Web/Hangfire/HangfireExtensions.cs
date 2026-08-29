using Hangfire;
using Hangfire.Dashboard;
using Hub.Infrastructure.Hangfire;

namespace Hub.Web.Hangfire;

static class HangfireExtensions
{
    const string DashboardPath = "/hangfire";

    extension(WebApplicationBuilder builder)
    {
        public void AddHangfireHost()
        {
            builder.Services.AddHangfireServer();
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
