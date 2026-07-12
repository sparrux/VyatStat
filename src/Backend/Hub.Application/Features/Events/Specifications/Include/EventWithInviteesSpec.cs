using Ardalis.Specification;
using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Specifications.Include;

sealed class EventWithInviteesSpec : Specification<Event>
{
    public EventWithInviteesSpec()
    {
        Query.Include(x => x.Invitees);
    }
}