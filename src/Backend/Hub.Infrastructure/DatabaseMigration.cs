using Hub.Infrastructure.Payments;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hub.Infrastructure;

public static class DatabaseMigration
{
    public static async Task MigrateDatabaseAsync(this IServiceProvider provider)
    {
        await using var scope = provider.CreateAsyncScope();

        var hub = scope.ServiceProvider.GetRequiredService<HubDbContext>();
        await hub.Database.MigrateAsync();

        var payments = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
        await payments.Database.MigrateAsync();
    }
}
