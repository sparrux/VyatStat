using Ardalis.Specification;
using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Specifications.Include;

sealed class EventWithDescriptionSpec : Specification<Event>
{
    public EventWithDescriptionSpec()
    {
        Query.Include(x => x.Description);
    }
}