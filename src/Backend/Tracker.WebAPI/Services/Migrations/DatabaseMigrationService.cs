using Microsoft.EntityFrameworkCore;
using Tracker.Infrastructure.Persistence;

namespace Tracker.WebAPI.Services.Migrations;

static class DatabaseMigrationService
{
    public static async Task MigrateAsync(WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }
}
