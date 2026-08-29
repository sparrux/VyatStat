using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Common.Specifications;
using Hub.Application.Features.Common.Specifications.Search;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.UpdateTitle;

sealed class UpdateTitleCommandHandler(
    IHubDbContext dbContext
) : IRequestHandler<UpdateTitleCommand, IdResponse>
{
    public async Task<Result<IdResponse>> Handle(
        UpdateTitleCommand command, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new GetByIdSpec<Event>(command.EventId))
            .FirstOrDefaultAsync(cancellationToken);

        if (ev is null) return Result.NotFound("Event not found by id");

        var request = command.Request;

        var updateResult = ev.UpdateTitle(request.NewTitle);

        if (!updateResult.IsSuccess) return updateResult.Map();
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success(new IdResponse(ev.Id));
    }
}