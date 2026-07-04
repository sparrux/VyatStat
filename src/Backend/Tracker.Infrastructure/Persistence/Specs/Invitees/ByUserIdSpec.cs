using Ardalis.Specification;
using Tracker.Domain.GroupEvents.Invitees;

namespace Tracker.Infrastructure.Persistence.Specs.Invitees;

sealed class ByUserIdSpec : Specification<GroupEventInvitee>
{
    public ByUserIdSpec(Guid userId)
    {
        Query.Where(i => i.UserId == userId);
    }
}