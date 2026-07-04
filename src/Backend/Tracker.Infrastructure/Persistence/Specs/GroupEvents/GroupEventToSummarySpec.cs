using Ardalis.Specification;
using Tracker.Application.Contracts.GroupEvents.Responses;
using Tracker.Domain.GroupEvents;

namespace Tracker.Infrastructure.Persistence.Specs.GroupEvents;

sealed class GroupEventToSummarySpec : Specification<GroupEvent, GroupEventSummaryResponse>
{
    public GroupEventToSummarySpec()
    {
        Query
            .AsNoTracking()
            .Select(x => new GroupEventSummaryResponse(
                x.Id,
                x.Title,
                x.EndDate,
                x.StartDate,
                x.Invitees.Count,
                x.Organizers.Count));
    }
}