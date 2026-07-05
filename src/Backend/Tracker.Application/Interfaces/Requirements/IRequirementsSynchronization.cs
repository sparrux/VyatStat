using Tracker.Domain.Events;
using Tracker.Domain.Events.Invitees;

namespace Tracker.Application.Interfaces.Requirements;

public interface IRequirementsSynchronization
{
    Task SynchronizeAsync(Event @event, CancellationToken ctk = default);
    Task SynchronizeAsync(Event @event, EventInvitee eventInvitee, CancellationToken ctk = default);
}