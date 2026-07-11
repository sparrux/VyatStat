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
        
        if (query.OrganizerUserId is { } organizerUserId)
            Query.Where(e => e.Organizers.Any(x => x.UserId == organizerUserId));
        
        if (query.InviteeUserId is { } inviteeUserId)
            Query.Where(e => e.Invitees.Any(x => x.UserId == inviteeUserId));
        
        if (query.State is { } state)
            Query.Where(e => e.State == state);
    }
}