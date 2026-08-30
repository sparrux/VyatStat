using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Common.Specifications.Search;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.DeleteDescription;

sealed class DeleteDescriptionCommandHandler(
    IHubDbContext dbContext
) : IRequestHandler<DeleteDescriptionCommand, IdResponse>
{
    public async Task<Result<IdResponse>> Handle(
        DeleteDescriptionCommand request, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new GetByIdSpec<Event>(request.EventId))
            .FirstOrDefaultAsync(cancellationToken);
        
        if (ev is null) return Result.NotFound("Event not found by id");

        var removeResult = ev.RemoveDescription();

        if (!removeResult.IsSuccess) return removeResult;
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new IdResponse(ev.Id));
    }
}