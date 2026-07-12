using Ardalis.Specification;
using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Specifications.Include;

sealed class EventWithOrganizersSpec : Specification<Event>
{
    public EventWithOrganizersSpec()
    {
        Query.Include(x => x.Organizers);
    }
}