using Ardalis.Specification;
using Tracker.Application.Contracts.Events.Requests;
using Tracker.Domain.Events;

namespace Tracker.Infrastructure.Persistence.Specs.Events.Search;

sealed class EventByFilterSpec : Specification<Event>
{
    public EventByFilterSpec(EventFilterRequest request)
    {
        if (request.GroupId is { } groupId)
            Query.Where(e => e.GroupEvents.Any(x => x.GroupId == groupId));
        
        if (request.OrganizerUserId is { } organizerUserId)
            Query.Where(e => e.Organizers.Any(x => x.UserId == organizerUserId));
        
        if (request.InviteeUserId is { } inviteeUserId)
            Query.Where(e => e.Invitees.Any(x => x.UserId == inviteeUserId));
        
        if (request.State is { } state)
            Query.Where(e => e.State == state);
    }
}