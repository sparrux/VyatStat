using Ardalis.Specification;
using Tracker.Domain.GroupEvents.Invitees;

namespace Tracker.Infrastructure.Persistence.Specs.Invitees;

sealed class WithRequirementsCompletionsSpec : Specification<GroupEventInvitee>
{
    public WithRequirementsCompletionsSpec()
    {
        Query.Include(i => i.RequirementCompletions);
    }
}