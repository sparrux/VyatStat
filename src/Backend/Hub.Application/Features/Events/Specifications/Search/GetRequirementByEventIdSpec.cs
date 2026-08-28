using Ardalis.Specification;
using Hub.Domain.Events.Requirements;

namespace Hub.Application.Features.Events.Specifications.Search;

sealed class GetRequirementByEventIdSpec : Specification<EventRequirement>
{
    public GetRequirementByEventIdSpec(Guid eventId)
    {
        Query.Where(x => x.EventId == eventId);
    }
}