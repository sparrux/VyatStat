using Ardalis.Specification;
using Tracker.Domain.Events;

namespace Tracker.Infrastructure.Persistence.Specs.Events.Include;

sealed class EventWithRequirementsSpec : Specification<Event>
{
    public EventWithRequirementsSpec()
    {
        Query.Include(x => x.Requirements);
    }
}