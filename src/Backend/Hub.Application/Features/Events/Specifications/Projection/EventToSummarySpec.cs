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
                x.DatesRange.EndDate,
                x.DatesRange.StartDate,
                x.Invitees.Count,
                x.Organizers.Count));
    }
}