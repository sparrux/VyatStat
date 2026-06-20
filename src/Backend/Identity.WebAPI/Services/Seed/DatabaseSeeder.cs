using Identity.WebAPI.Persistence;

namespace Identity.WebAPI.Services.Seed;

static class DatabaseSeeder
{
    public static async Task SeedDatabaseAsync(WebApplication web)
    {
        await using var scope = web.Services.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
}