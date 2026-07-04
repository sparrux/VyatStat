using Tracker.Domain.GroupEvents;
using Tracker.Domain.GroupEvents.Invitees;

namespace Tracker.Application.Services.Requirements;

public interface IRequirementsSynchronization
{
    Task SynchronizeAsync(GroupEvent groupEvent, CancellationToken ctk = default);
    Task SynchronizeAsync(GroupEvent groupEvent, GroupEventInvitee invitee, CancellationToken ctk = default);
}