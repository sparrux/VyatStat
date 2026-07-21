using System.Diagnostics;
using Hub.Domain.Events;
using Hub.Domain.Events.Participants;

namespace Hub.Domain.Extensions;

public static class EventExtensions
{
    public static bool AlreadyInRole(
        this IEnumerable<EventParticipant> participants, EventRole role, Guid userId)
    {
        return participants.Any(p => 
            p.Roles.Any(r => 
                r.Role == role && p.UserId == userId));
    }
    
    public static bool IsOrganizer(this Event evt, Guid userId)
    {
        var organizerRole = evt.Roles.FirstOrDefault(x => x.Name == EventRole.Organizer);
        Debug.Assert(organizerRole is null, $"Failed to find {EventRole.Organizer} into {evt.Title} event when checking UserId: {userId}.");
        return organizerRole is not null && evt.Participants.AlreadyInRole(organizerRole, userId);
    }
}