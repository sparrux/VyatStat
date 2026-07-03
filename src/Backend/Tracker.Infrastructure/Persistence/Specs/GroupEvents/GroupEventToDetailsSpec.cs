using Ardalis.Specification;
using Tracker.Application.Contracts.Common.Responses;
using Tracker.Application.Contracts.Event.Responses;
using Tracker.Domain.GroupEvents.Events;

namespace Tracker.Infrastructure.Persistence.Specs.GroupEvents;

sealed class GroupEventToDetailsSpec : Specification<GroupEvent, GroupEventDetailsResponse>
{
    public GroupEventToDetailsSpec()
    {
        Query
            .AsNoTracking()
            .Select(x => new GroupEventDetailsResponse(
                x.Id,
                x.GroupId,
                x.Title,
                new FormatTextResponse(x.Description.Text, x.Description.Format),
                x.EndDate,
                x.StartDate,
                x.State,
                x.Location != null ?
                    new LocationResponse(
                        x.Location.Id,
                        x.Location.Location.Name,
                        x.Location.Location.Latitude,
                        x.Location.Location.Longitude)
                    : null
            ));
    }
}