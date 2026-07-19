using Ardalis.Specification;
using Hub.Application.Features.Events.Contracts;
using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Specifications.Projection;

sealed class EventToSummarySpec : Specification<Event, EventSummaryResponse>
{
    public EventToSummarySpec()
    {
        Query
            .AsNoTracking()
            .Select(x => new EventSummaryResponse(
                x.Id,
                x.Title,
                x.State,
                x.DatesRange.EndDate,
                x.DatesRange.StartDate,
                x.Location != null,
                x.Participants.Count,
                x.Organizers.Count,
                x.Requirements.Count,
                x.Goals.Count));
    }
}