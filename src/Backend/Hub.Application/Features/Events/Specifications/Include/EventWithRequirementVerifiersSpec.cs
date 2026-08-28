using Ardalis.Specification;
using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Specifications.Include;

public sealed class EventWithRequirementVerifiersSpec : Specification<Event>
{
    public EventWithRequirementVerifiersSpec()
    {
        Query
            .Include(e => e.Requirements)
            .ThenInclude(x => x.Verifiers);
    }
}