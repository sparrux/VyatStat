using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Abstractions;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Common.Specifications.Search;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.UpdateState;

sealed class UpdateStateCommandHandler(
    IHubDbContext dbContext,
    IEventScheduler eventScheduler,
    TimeProvider time
) : IRequestHandler<UpdateStateCommand, IdResponse>
{
    public async Task<Result<IdResponse>> Handle(
        UpdateStateCommand command, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new GetByIdSpec<Event>(command.EventId))
            .FirstOrDefaultAsync(cancellationToken);

        if (ev is null) return Result.NotFound("Event not found by id");
        
        if (CheckNewEventState(ev, command.NewState) is { IsSuccess: false } checking)
            return checking;

        var updateResult = ev.UpdateState(command.NewState);
        if (!updateResult.IsSuccess) return updateResult.Map();

        await dbContext.SaveChangesAsync(cancellationToken);

        if (Event.IsFinished(ev.State))
            await eventScheduler.DeleteAsync(ev, cancellationToken);
        else if (Event.IsOngoing(ev.State))
            await eventScheduler.ScheduleAsync(ev, cancellationToken);

        return Result.Success(new IdResponse(ev.Id));
    }

    Result CheckNewEventState(Event ev, EventState newState)
    {
        if (ev.State == newState)
            return Result.Error("Cannot update event state to the same state");

        if (newState is EventState.InProgress
            && time.GetUtcNow() < ev.DatesRange.StartDate)
            return Result.Error("Cannot set event as in progress before start date");

        if (newState is EventState.Completed
            && ev.State is not EventState.InProgress)
            return Result.Error("Cannot complete event that is not in progress");

        return Result.Success();
    }
}
