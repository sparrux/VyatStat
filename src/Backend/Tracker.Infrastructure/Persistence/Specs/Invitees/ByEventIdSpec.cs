using Ardalis.Specification;
using Tracker.Domain.GroupEvents.Events;

namespace Tracker.Infrastructure.Persistence.Specs.Invitees;

sealed class ByEventIdSpec : Specification<GroupEventInvitee>
{
    public ByEventIdSpec(Guid eventId)
    {
        Query.Where(x => x.EventId == eventId);
    }
}