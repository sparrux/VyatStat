using Ardalis.Specification;
using Tracker.Domain.Events.Invitees;

namespace Tracker.Infrastructure.Persistence.Specs.Invitees.Search;

sealed class InviteeByUserIdSpec : Specification<EventInvitee>
{
    public InviteeByUserIdSpec(Guid userId)
    {
        Query.Where(i => i.UserId == userId);
    }
}