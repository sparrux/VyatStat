using Tracker.Domain;
using Tracker.Infrastructure.Persistence;

namespace Tracker.WebAPI.Services.Seeders;

static class DatabaseSeeder
{
    static readonly Guid AdminUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    static readonly Guid RegularUserId1 = Guid.Parse("22222222-2222-2222-2222-222222222222");
    static readonly Guid RegularUserId2 = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static async Task SeedAsync(WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        if (!context.Users.Any(x => x.Id == AdminUserId))
        {
            var user1 = User.Create(AdminUserId, "admin");
            await context.AddAsync(user1.Value);
        }

        if (!context.Users.Any(x => x.Id == RegularUserId1))
        {
            var user2 = User.Create(RegularUserId1, "user-1");
            await context.AddAsync(user2.Value);
        }

        if (!context.Users.Any(x => x.Id == RegularUserId2))
        {
            var user3 = User.Create(RegularUserId2, "user-2");
            await context.AddAsync(user3.Value);
        }
        
        await context.SaveChangesAsync();
    }
}
