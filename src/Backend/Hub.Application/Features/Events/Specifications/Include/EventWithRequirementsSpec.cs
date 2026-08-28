using Ardalis.Specification;
using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Specifications.Include;

sealed class EventWithRequirementsSpec : Specification<Event>
{
    public EventWithRequirementsSpec()
    {
        Query.Include(x => x.Requirements);
    }
}