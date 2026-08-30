using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Abstractions;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Common.Specifications.Search;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.ReconcileState;

sealed class ReconcileStateCommandHandler(
    IHubDbContext dbContext,
    TimeProvider time
) : IRequestHandler<ReconcileStateCommand, IdResponse>
{
    public async Task<Result<IdResponse>> Handle(
        ReconcileStateCommand command, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new GetByIdSpec<Event>(command.EventId))
            .FirstOrDefaultAsync(cancellationToken);

        if (ev is null) return Result.NotFound("Event not found by id");

        if (Event.IsFinished(ev.State))
            return Result.Success(new IdResponse(ev.Id));

        var next = ResolveNextState(ev, time.GetUtcNow());
        if (next is null)
            return Result.Success(new IdResponse(ev.Id));

        var update = ev.UpdateState(next.Value);
        if (!update.IsSuccess) return update.Map();

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(new IdResponse(ev.Id));
    }

    static EventState? ResolveNextState(Event ev, DateTimeOffset utcNow)
    {
        if (ev.DatesRange.EndDate <= utcNow && Event.IsOngoing(ev.State))
            return EventState.Completed;

        if (ev.DatesRange.StartDate <= utcNow && Event.IsReadyForStarting(ev.State))
            return EventState.InProgress;

        return null;
    }
}
