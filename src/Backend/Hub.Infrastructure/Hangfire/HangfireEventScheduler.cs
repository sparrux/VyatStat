using Hangfire;
using Hangfire.States;
using Hub.Application.Abstractions;
using Hub.Domain.Events;
using Hub.Infrastructure.Hangfire.Jobs;

namespace Hub.Infrastructure.Hangfire;

public sealed class HangfireEventScheduler(
    IBackgroundJobClient jobs,
    JobStorage storage
) : IEventScheduler
{
    const string StartJob = "start";
    const string EndJob = "end";

    public Task ScheduleAsync(Event evt, CancellationToken cancellationToken)
    {
        DeleteJobs(evt.Id);

        var startJobId = jobs.Schedule<IEventStateJobs>(
            processor => processor.TransitionToInProgressAsync(evt.Id),
            evt.DatesRange.StartDate);

        var endJobId = jobs.Schedule<IEventStateJobs>(
            processor => processor.TransitionToCompletedAsync(evt.Id),
            evt.DatesRange.EndDate);

        using var connection = storage.GetConnection();
        using var transaction = connection.CreateWriteTransaction();
        transaction.SetRangeInHash(JobKey(evt.Id),
        [
            new KeyValuePair<string, string>(StartJob, startJobId),
            new KeyValuePair<string, string>(EndJob, endJobId)
        ]);
        transaction.Commit();

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Event evt, CancellationToken cancellationToken)
    {
        DeleteJobs(evt.Id);
        return Task.CompletedTask;
    }

    void DeleteJobs(Guid eventId)
    {
        var key = JobKey(eventId);
        using var connection = storage.GetConnection();
        var entries = connection.GetAllEntriesFromHash(key);

        if (entries is not null)
            foreach (var jobId in entries.Values.Where(job => !string.IsNullOrWhiteSpace(job)))
                jobs.ChangeState(jobId, new DeletedState());

        using var transaction = connection.CreateWriteTransaction();
        transaction.RemoveHash(key);
        transaction.Commit();
    }

    static string JobKey(Guid eventId) => $"event-scheduler:{eventId:D}";
}
