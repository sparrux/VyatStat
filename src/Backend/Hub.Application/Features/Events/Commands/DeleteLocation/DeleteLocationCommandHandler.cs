using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Common.Specifications;
using Hub.Application.Features.Common.Specifications.Search;
using Hub.Application.Features.Events.Specifications.Include;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.DeleteLocation;

sealed class DeleteLocationCommandHandler(
    HubDbContext dbContext
) : IRequestHandler<DeleteLocationCommand, IdResponse>
{
    public async Task<Result<IdResponse>> Handle(
        DeleteLocationCommand request, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new GetByIdSpec<Event>(request.EventId))
            .WithSpecification(new EventWithLocationSpec())
            .FirstOrDefaultAsync(cancellationToken);
        
        if (ev is null) return Result.NotFound("Event not found by id");

        var removeResult = ev.RemoveLocation();
        
        if (!removeResult.IsSuccess) return removeResult;
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new IdResponse(ev.Id));
    }
}