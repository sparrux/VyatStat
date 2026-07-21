using Ardalis.Specification;
using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Specifications.Include;

sealed class EventWithRequirementCompletionsSpec : Specification<Event>
{
    public EventWithRequirementCompletionsSpec()
    {
        Query
            .Include(x => x.Participants)
            .ThenInclude(x => x.Requirements);
    }
}