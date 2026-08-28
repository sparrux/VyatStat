using Hangfire;
using Hub.Domain.Events;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Web.Hangfire;

public sealed class EventStateTransitionProcessor(
    HubDbContext dbContext,
    TimeProvider time,
    ILogger<EventStateTransitionProcessor> logger)
{
    [AutomaticRetry(Attempts = 3)]
    [DisableConcurrentExecution(timeoutInSeconds: 60)]
    [JobDisplayName("{1}: start → InProgress")]
    public Task TransitionToInProgressAsync(Guid eventId, string eventTitle) =>
        ReconcileAsync(eventId, eventTitle);

    [AutomaticRetry(Attempts = 3)]
    [DisableConcurrentExecution(timeoutInSeconds: 60)]
    [JobDisplayName("{1}: end → Completed")]
    public Task TransitionToCompletedAsync(Guid eventId, string eventTitle) =>
        ReconcileAsync(eventId, eventTitle);

    async Task ReconcileAsync(Guid eventId, string eventTitle)
    {
        var evt = await dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
        if (evt is null || Event.IsFinished(evt.State))
            return;

        var utcNow = time.GetUtcNow();
        EventState? next = null;

        if (evt.DatesRange.EndDate <= utcNow &&
            evt.State is EventState.RegistrationOpen or EventState.RegistrationClosed or EventState.InProgress)
            next = EventState.Completed;
        else if (evt.DatesRange.StartDate <= utcNow &&
                 evt.State is EventState.RegistrationOpen or EventState.RegistrationClosed)
            next = EventState.InProgress;

        if (next is null)
            return;

        var update = evt.UpdateState(next.Value);
        if (!update.IsSuccess)
        {
            logger.LogError(
                "Error while updating event {EventTitle} ({EventId}): {@Errors}",
                eventTitle,
                evt.Id,
                update.Errors);
            return;
        }

        await dbContext.SaveChangesAsync();
        logger.LogInformation(
            "Event {EventTitle} ({EventId}) transitioned to {State}",
            eventTitle,
            evt.Id,
            next.Value);
    }
}
