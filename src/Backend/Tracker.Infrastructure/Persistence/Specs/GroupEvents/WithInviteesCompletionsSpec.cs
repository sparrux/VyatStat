using Ardalis.Specification;
using Tracker.Domain.GroupEvents;

namespace Tracker.Infrastructure.Persistence.Specs.GroupEvents;

sealed class WithInviteesCompletionsSpec : Specification<GroupEvent>
{
    public WithInviteesCompletionsSpec()
    {
        Query
            .Include(x => x.Invitees)
            .ThenInclude(x => x.RequirementCompletions);
    }
}