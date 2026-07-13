using Ardalis.Specification;
using Hub.Domain.Events.Invitees;

namespace Hub.Application.Features.Events.Specifications.Search;

sealed class GetInviteeByEventIdSpec : Specification<EventInvitee>
{
    public GetInviteeByEventIdSpec(Guid eventId)
    {
        Query.Where(x => x.EventId == eventId);
    }
}