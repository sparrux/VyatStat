using Ardalis.Specification;
using Hub.Application.Features.Events.Queries.Get;
using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Specifications.Search;

sealed class GetEventByQuerySpec : Specification<Event>
{
    public GetEventByQuerySpec(GetEventQuery query)
    {
        if (query.GroupId is { } groupId)
            Query.Where(e => e.GroupEvents.Any(x => x.GroupId == groupId));
        
        if (query.ParticipantUserId is { } participantUserId)
            Query.Where(e => e.Participants.Any(x => x.UserId == participantUserId));
        
        if (query.State is { } state)
            Query.Where(e => e.State == state);
    }
}