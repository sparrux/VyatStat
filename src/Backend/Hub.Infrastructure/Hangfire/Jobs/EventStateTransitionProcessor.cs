using Hangfire;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Events.Commands.ReconcileState;
using Hub.Application.Pipelines;
using Microsoft.Extensions.Logging;

namespace Hub.Infrastructure.Hangfire.Jobs;

public sealed class EventStateTransitionProcessor(
    IRequestHandler<ReconcileStateCommand, IdResponse> handler,
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
        var result = await handler.Handle(new ReconcileStateCommand(eventId), CancellationToken.None);
        if (result.IsSuccess)
            return;

        logger.LogError(
            "Failed to reconcile event {EventId}: {@Errors}",
            eventId,
            result.Errors);
    }
}
