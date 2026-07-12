using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Common.Specifications;
using Hub.Application.Features.Events.Specifications.Include;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Domain.ValueObjects;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.UpdateLocation;

sealed class UpdateLocationCommandHandler(
    HubDbContext dbContext
) : IRequestHandler<UpdateLocationCommand, IdResponse>
{
    public async Task<Result<IdResponse>> Handle(
        UpdateLocationCommand command, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new EventWithLocationSpec())
            .WithSpecification(new GetByIdSpec<Event>(command.EventId))
            .FirstOrDefaultAsync(cancellationToken);
        
        if (ev is null) return Result.NotFound("Event not found by id");

        var request = command.Request;

        var updateResult = ev.UpdateLocation(
            request.Name, new Coordinates(request.Longitude, request.Latitude));

        if (!updateResult.IsSuccess) return updateResult;

        await dbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success(new IdResponse(ev.Id));
    }
}