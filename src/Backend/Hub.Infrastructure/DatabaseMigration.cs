using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hub.Infrastructure;

public static class DatabaseMigration
{
    public static async Task MigrateDatabaseAsync(this IServiceProvider provider)
    {
        await using var scope = provider.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<HubDbContext>();
        await context.Database.MigrateAsync();
    }
}
