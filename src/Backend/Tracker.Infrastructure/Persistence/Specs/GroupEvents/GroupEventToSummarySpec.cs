using Ardalis.Specification;
using Tracker.Application.Contracts.Event.Responses;
using Tracker.Domain.GroupEvents.Events;

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
                x.Invitees.Count));
    }
}