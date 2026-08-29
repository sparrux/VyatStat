using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Common.Specifications;
using Hub.Application.Features.Common.Specifications.Search;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Domain.ValueObjects;
using Hub.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.UpdateDates;

sealed class UpdateDatesCommandHandler(
    IHubDbContext dbContext
) : IRequestHandler<UpdateDatesCommand, IdResponse>
{
    public async Task<Result<IdResponse>> Handle(
        UpdateDatesCommand command, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new GetByIdSpec<Event>(command.EventId))
            .FirstOrDefaultAsync(cancellationToken);

        if (ev is null) return Result.NotFound("Event not found by id");

        var updateResult = ev.UpdateDates(
            new DatesRange(command.Request.StartDate, command.Request.EndDate));

        if (!updateResult.IsSuccess) return updateResult;
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success(new IdResponse(ev.Id));
    }
}