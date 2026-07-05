using Ardalis.Specification;
using Tracker.Domain.Events.Invitees;

namespace Tracker.Infrastructure.Persistence.Specs.Invitees.Search;

sealed class InviteeByEventIdSpec : Specification<EventInvitee>
{
    public InviteeByEventIdSpec(Guid eventId)
    {
        Query.Where(x => x.EventId == eventId);
    }
}