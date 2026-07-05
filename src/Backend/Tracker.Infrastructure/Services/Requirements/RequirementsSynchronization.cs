using Tracker.Application.Interfaces.Requirements;
using Tracker.Domain.Events;
using Tracker.Domain.Events.Invitees;
using Tracker.Infrastructure.Persistence;

namespace Tracker.Infrastructure.Services.Requirements;

public sealed class RequirementsSynchronization(AppDbContext context) : IRequirementsSynchronization
{
    public Task SynchronizeAsync(Event @event, CancellationToken ctk = default)
    {
        Synchronize(@event, @event.Invitees.ToArray());
        return context.SaveChangesAsync(ctk);
    }

    public Task SynchronizeAsync(Event @event, EventInvitee eventInvitee, CancellationToken ctk = default)
    {
        Synchronize(@event, [eventInvitee]);
        return context.SaveChangesAsync(ctk);
    }
    
    static void Synchronize(Event @event, EventInvitee[] inviteesSelection)
    {
        var requirements = @event.Requirements;
        
        foreach (var invitee in inviteesSelection)
        {
            foreach (var requirement in requirements)
            {
                var completion = invitee.RequirementCompletions
                    .FirstOrDefault(r => r.Requirement == requirement);
            
                if (completion is null)
                    invitee.AddCompletion(requirement).ToResult();
            }
            
            foreach (var completion in invitee.RequirementCompletions)
            {
                if (requirements.All(r => r.Id != completion.Requirement.Id))
                    invitee.RemoveCompletion(completion);
            }
        }
    }
}