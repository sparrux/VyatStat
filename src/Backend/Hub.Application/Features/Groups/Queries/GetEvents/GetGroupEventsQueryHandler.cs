using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Common.Specifications.Ordering;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Events.Specifications.Projection;
using Hub.Application.Features.Groups.Specifications.Search;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Groups.Queries.GetEvents;

sealed class GetGroupEventsQueryHandler(
    IHubDbContext dbContext
) : IRequestHandler<GetGroupEventsQuery, ListResponse<EventSummaryResponse>>
{
    public async Task<Result<ListResponse<EventSummaryResponse>>> Handle(
        GetGroupEventsQuery request, CancellationToken cancellationToken)
    {
        var groupBySpec = new GetEventByGroupSpec(request.GroupId);
        
        var events = await dbContext.GroupEvent
            .WithSpecification(groupBySpec)
            .WithSpecification(new GroupEventDatesSelectionSpec(request.FromDate, request.ToDate))
            .Select(x => x.Event)
            .WithSpecification(new OrderByCreatedAtSpec<Event>(true))
            .WithSpecification(new EventToSummarySpec())
            .ToListAsync(cancellationToken);
        
        return Result.Success(new ListResponse<EventSummaryResponse>(
            events,
            await dbContext.GroupEvent
                .WithSpecification(groupBySpec)
                .CountAsync(cancellationToken)));
    }
}