using Hub.Domain;
using Hub.Domain.Events;
using Hub.Domain.ValueObjects;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Web.Services.Seeders;

sealed class UsersSeeder(HubDbContext dbContext) : ISeeder
{
    public async Task Seed(CancellationToken ctk)
    {
        if (!await dbContext.Users.AnyAsync(x => x.Nickname == "john", cancellationToken: ctk))
            await dbContext.AddAsync(User.Create(Guid.NewGuid(), "john").Value, ctk);
        
        if (!await dbContext.Users.AnyAsync(x => x.Nickname == "sam", cancellationToken: ctk))
            await dbContext.AddAsync(User.Create(Guid.NewGuid(), "sam").Value, ctk);
        
        if (!await dbContext.Users.AnyAsync(x => x.Nickname == "barbara", cancellationToken: ctk))
            await dbContext.AddAsync(User.Create(Guid.NewGuid(), "barbara").Value, ctk);
        
        await dbContext.SaveChangesAsync(ctk);

        var users = await dbContext.Users.ToListAsync(ctk);
        await SeedEvents(users.ToArray(), ctk);
    }

    async Task SeedEvents(User[] users, CancellationToken ctk)
    {
        return;
        
        var organizer = users[0];
        var user1 = users[1];
        var user2 = users[2];
        
        if (!await dbContext.Events.AnyAsync(x => x.Title == "First Sample Event", cancellationToken: ctk))
        {
            var evt = Event.CreateDraft(
                organizer, 
                "First Sample Event", 
                new DatesRange(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(1))).Value;

            evt.UpdateState(EventState.RegistrationOpen);
            
            var cgRole = evt.AddRole("GC", isSealed: true).Value;
            var supportRole = evt.AddRole("Support", isSealed: false).Value;
            
            var participant1 = evt.AddParticipant(user1).Value;
            var participant2 = evt.AddParticipant(user2).Value;

            evt.AddParticipantRole(cgRole, participant1);
            evt.AddParticipantRole(supportRole, participant2);
            
            var goal1 = evt.AddGoal("Sample Event Goal #1").Value;
            var goal2 = evt.AddGoal("Sample Event Goal #2").Value;

            var requirement1 = evt.AddRequirement("Sample Event Requirement #1", "Description #1").Value;
            var requirement2 = evt.AddRequirement("Sample Event Requirement #2", "Description #2").Value;

            evt.AddRequirementRoleVerifier(requirement1, cgRole, isRequired: false);
            evt.AddRequirementRoleVerifier(requirement2, supportRole, isRequired: false);
            
            await dbContext.AddAsync(evt, ctk);
            await dbContext.SaveChangesAsync(ctk);
        }
    }
}