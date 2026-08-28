using Ardalis.Result;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Common.Specifications;
using Hub.Application.Features.Common.Specifications.Search;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Events.Specifications.Projection;
using Hub.Application.Features.Events.Specifications.Search;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Queries.Get;

sealed class GetEventQueryHandler(
    HubDbContext dbContext
) : IRequestHandler<GetEventQuery, ListResponse<EventSummaryResponse>>
{
    public async Task<Result<ListResponse<EventSummaryResponse>>> Handle(
        GetEventQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetEventByQuerySpec(request);
        
        var result = await dbContext.Events
            .WithSpecification(new ListSelectionSpec<Event>(request.Take, request.Skip))
            .WithSpecification(spec.WithProjectionOf(new EventToSummarySpec()))
            .ToListAsync(cancellationToken);

        return Result.Success(
            new ListResponse<EventSummaryResponse>(
                result,
                await dbContext.Events
                    .WithSpecification(spec)
                    .CountAsync(cancellationToken)));
    }
}