using Ardalis.Specification;
using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Specifications.Include;

sealed class EventWithRequirementsCompletionsSpec : Specification<Event>
{
    public EventWithRequirementsCompletionsSpec()
    {
        Query
            .Include(x => x.Invitees)
            .ThenInclude(x => x.RequirementCompletions);
    }
}