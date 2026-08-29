using Hangfire;

namespace Hub.Infrastructure.Hangfire.Jobs;

public interface IEventStateJobs
{
    [AutomaticRetry(Attempts = 3)]
    [DisableConcurrentExecution(timeoutInSeconds: 60)]
    [JobDisplayName("{0}: start → InProgress")]
    Task TransitionToInProgressAsync(Guid eventId);

    [AutomaticRetry(Attempts = 3)]
    [DisableConcurrentExecution(timeoutInSeconds: 60)]
    [JobDisplayName("{0}: end → Completed")]
    Task TransitionToCompletedAsync(Guid eventId);
}
