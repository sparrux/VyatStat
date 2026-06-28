using Tracker.Domain;
using Tracker.Domain.GroupEvents.Events;
using Tracker.Domain.Groups;
using Tracker.Infrastructure.Persistence;

namespace Tracker.WebAPI.Services.Seed;

static class DatabaseSeeder
{
    public static async Task SeedAsync(WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user1 = User.Create("admin");
        var user2 = User.Create("user");
        
        var group = Group.Create("Sample");
        
        var member1 = group.Value.AddMember(user1.Value);
        var member2 = group.Value.AddMember(user2.Value);

        var @event = group.Value.AddEvent("Event #1", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(1));
        
        var req1 = @event.Value.AddRequirement(
            GroupEventRequirement
                .Create("Requirement #1", "Text 1", true, 1).Value);
        var req2 = @event.Value.AddRequirement(
            GroupEventRequirement
                .Create("Requirement #2", "Text 2", true, 2).Value);
        
        var invitee1 = @event.Value.AddInvitee(GroupEventInvitee.Create(user1.Value).Value);
        var invitee2 = @event.Value.AddInvitee(GroupEventInvitee.Create(user2.Value).Value);

        await context.AddAsync(group.Value);
        await context.SaveChangesAsync();
    }
}