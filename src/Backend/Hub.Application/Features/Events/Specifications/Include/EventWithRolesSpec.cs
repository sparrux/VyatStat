using Ardalis.Specification;
using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Specifications.Include;

sealed class EventWithRolesSpec : Specification<Event>
{
    public EventWithRolesSpec()
    {
        Query.Include(x => x.Roles);
    }
}
