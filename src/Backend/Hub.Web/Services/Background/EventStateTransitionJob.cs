using System.Linq.Expressions;
using Hub.Domain.Events;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Web.Services.Background;

sealed class EventStateTransitionJob(
    ILogger<EventStateTransitionJob> logger,
    IServiceScopeFactory scopes,
    TimeProvider time
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10), time);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await using var scope = scopes.CreateAsyncScope();
                    await ExecuteTransitionAsync(scope.ServiceProvider, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while executing event state transition");
                }
            }
        }
        finally
        {
            logger.LogWarning("Event state transition service stopped");
        }
    }
    
    async Task ExecuteTransitionAsync(IServiceProvider services, CancellationToken stoppingToken)
    {
        var context = services.GetRequiredService<HubDbContext>();
        
        var utcNow = time.GetUtcNow();

        var ongoing = await SetEventsStateAsync(context, x =>
            x.DatesRange.StartDate <= utcNow &&
            (x.State == EventState.RegistrationClosed ||
             x.State == EventState.RegistrationOpen), EventState.InProgress, stoppingToken);
        
        var finished = await SetEventsStateAsync(context, x =>
            x.DatesRange.EndDate <= utcNow &&
            x.State == EventState.InProgress, EventState.Completed, stoppingToken);

        var changes = ongoing + finished;

        if (changes > 0)
        {
            await context.SaveChangesAsync(stoppingToken);
            logger.LogInformation("Events state transitioned: {Changes}", changes);
        }
    }

    async Task<int> SetEventsStateAsync(
        HubDbContext context, 
        Expression<Func<Event, bool>> predicate, 
        EventState state, 
        CancellationToken stoppingToken)
    {
        var statefulEvents = await context.Events
            .Where(predicate)
            .ToListAsync(stoppingToken);

        var failures = 0;
        
        statefulEvents.ForEach(evt =>
        {
            var update = evt.UpdateState(state);

            if (update.IsSuccess) return;
            
            failures++;
            logger.LogError("Error while updating event state of event {EventId}: {@Errors}", evt.Id, update.Errors);
        });
        
        return statefulEvents.Count - failures;
    }
}