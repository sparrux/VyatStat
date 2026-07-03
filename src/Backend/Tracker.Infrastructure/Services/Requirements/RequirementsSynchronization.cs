using Tracker.Application.Services.Requirements;
using Tracker.Domain.GroupEvents.Events;
using Tracker.Infrastructure.Persistence;

namespace Tracker.Infrastructure.Services.Requirements;

public sealed class RequirementsSynchronization(AppDbContext context) : IRequirementsSynchronization
{
    public Task SynchronizeAsync(GroupEvent groupEvent, CancellationToken ctk = default)
    {
        Synchronize(groupEvent, groupEvent.Invitees.ToArray());
        return context.SaveChangesAsync(ctk);
    }

    public Task SynchronizeAsync(GroupEvent groupEvent, GroupEventInvitee invitee, CancellationToken ctk = default)
    {
        Synchronize(groupEvent, [invitee]);
        return context.SaveChangesAsync(ctk);
    }
    
    static void Synchronize(GroupEvent groupEvent, GroupEventInvitee[] inviteesSelection)
    {
        var requirements = groupEvent.Requirements;
        
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