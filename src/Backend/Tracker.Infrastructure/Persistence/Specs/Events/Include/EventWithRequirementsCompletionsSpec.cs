using Ardalis.Specification;
using Tracker.Domain.Events;

namespace Tracker.Infrastructure.Persistence.Specs.Events.Include;

sealed class EventWithRequirementsCompletionsSpec : Specification<Event>
{
    public EventWithRequirementsCompletionsSpec()
    {
        Query
            .Include(x => x.Invitees)
            .ThenInclude(x => x.RequirementCompletions);
    }
}