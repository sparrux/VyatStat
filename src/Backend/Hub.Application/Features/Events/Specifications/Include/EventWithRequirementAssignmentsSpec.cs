using Ardalis.Specification;
using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Specifications.Include;

sealed class EventWithRequirementAssignmentsSpec : Specification<Event>
{
    public EventWithRequirementAssignmentsSpec()
    {
        Query
            .Include(x => x.Participants)
            .ThenInclude(x => x.Requirements);
    }
}