using Microsoft.EntityFrameworkCore;
using Tracker.Infrastructure.Persistence;

namespace Tracker.WebAPI;

static class DependencyInjection
{
    public static void AddWebServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        builder.Services.AddControllers();

        builder.AddEntityFrameworkCore();
    }

    static void AddEntityFrameworkCore(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("TrackerDb")));
    }
}
