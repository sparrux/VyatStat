using Ardalis.Specification;
using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Specifications.Include;

sealed class EventWithLocationSpec : Specification<Event>
{
    public EventWithLocationSpec()
    {
        Query.Include(x => x.Location);
    }
}