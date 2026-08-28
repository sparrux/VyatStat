using Ardalis.Specification;
using Hub.Domain.Events.Participants;

namespace Hub.Application.Features.Events.Specifications.Search;

sealed class GetParticipantByEventIdSpec : Specification<EventParticipant>
{
    public GetParticipantByEventIdSpec(Guid eventId)
    {
        Query.Where(x => x.EventId == eventId);
    }
}
