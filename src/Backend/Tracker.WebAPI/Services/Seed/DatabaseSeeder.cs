using Microsoft.EntityFrameworkCore;
using Tracker.Domain;
using Tracker.Domain.GroupEvents.Events;
using Tracker.Domain.Groups;
using Tracker.Infrastructure.Persistence;

namespace Tracker.WebAPI.Services.Seed;

static class DatabaseSeeder
{
    static readonly Guid AdminUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    static readonly Guid RegularUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static async Task SeedAsync(WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (await context.Users.AnyAsync())
            return;

        var user1 = User.Create(AdminUserId, "admin");
        var user2 = User.Create(RegularUserId, "user");

        var group = Group.Create("Sample");

        group.Value.AddMember(user1.Value);
        group.Value.AddMember(user2.Value);

        var @event = group.Value.AddEvent("Event #1", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(1));

        @event.Value.AddRequirement(
            GroupEventRequirement
                .Create("Requirement #1", "Text 1", true, 1).Value);
        @event.Value.AddRequirement(
            GroupEventRequirement
                .Create("Requirement #2", "Text 2", true, 2).Value);

        @event.Value.AddInvitee(GroupEventInvitee.Create(user1.Value).Value);
        @event.Value.AddInvitee(GroupEventInvitee.Create(user2.Value).Value);

        await context.AddAsync(group.Value);
        await context.SaveChangesAsync();
    }
}
