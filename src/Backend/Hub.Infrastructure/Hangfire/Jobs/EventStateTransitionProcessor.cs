using Hangfire;
using Hub.Application.Abstractions;
using Hub.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hub.Infrastructure.Hangfire.Jobs;

public sealed class EventStateTransitionProcessor(
    IHubDbContext dbContext,
    TimeProvider time,
    ILogger<EventStateTransitionProcessor> logger
) : IEventStateJobs
{
    [AutomaticRetry(Attempts = 3)]
    [DisableConcurrentExecution(timeoutInSeconds: 60)]
    [JobDisplayName("{0}: start → InProgress")]
    public Task TransitionToInProgressAsync(Guid eventId) =>
        ReconcileAsync(eventId);

    [AutomaticRetry(Attempts = 3)]
    [DisableConcurrentExecution(timeoutInSeconds: 60)]
    [JobDisplayName("{0}: end → Completed")]
    public Task TransitionToCompletedAsync(Guid eventId) =>
        ReconcileAsync(eventId);

    async Task ReconcileAsync(Guid eventId)
    {
        var ev = await dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
        if (ev is null || Event.IsFinished(ev.State))
            return;

        var utcNow = time.GetUtcNow();
        EventState? next = null;

        if (ev.DatesRange.EndDate <= utcNow && Event.IsOngoing(ev.State))
            next = EventState.Completed;
        else if (ev.DatesRange.StartDate <= utcNow &&
                 ev.State is EventState.RegistrationOpen or EventState.RegistrationClosed)
            next = EventState.InProgress;

        if (next is null)
            return;

        var update = ev.UpdateState(next.Value);
        if (!update.IsSuccess)
        {
            logger.LogError(
                "Failed to reconcile event {EventTitle} ({EventId}): {@Errors}",
                ev.Title,
                eventId,
                update.Errors);
            return;
        }

        await dbContext.SaveChangesAsync();
        logger.LogInformation(
            "Event {EventTitle} ({EventId}) transitioned to {State}",
            ev.Title,
            eventId,
            next.Value);
    }
}
