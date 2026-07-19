using Ardalis.Specification;
using Hub.Domain.Events.Participants;

namespace Hub.Application.Features.Events.Specifications.Search;

sealed class GetInviteeByEventIdSpec : Specification<EventParticipant>
{
    public GetInviteeByEventIdSpec(Guid eventId)
    {
        Query.Where(x => x.EventId == eventId);
    }
}