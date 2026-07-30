using Ardalis.Result;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Specifications;
using Hub.Application.Features.Common.Specifications.Search;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Events.Specifications.Projection;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Queries.GetById;

sealed class GetEventByIdQueryHandler(
    HubDbContext dbContext
) : IRequestHandler<GetEventByIdQuery, EventDetailsResponse>
{
    public async Task<Result<EventDetailsResponse>> Handle(
        GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new GetByIdSpec<Event>(request.EventId)
                .WithProjectionOf(new EventToDetailsSpec()))
            .FirstOrDefaultAsync(cancellationToken);

        return ev is null
            ? Result.NotFound("Event not found by id")
            : Result.Success(ev);
    }
}