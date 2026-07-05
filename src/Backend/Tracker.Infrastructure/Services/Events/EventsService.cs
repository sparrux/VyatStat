using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tracker.Application.Contracts.Events.Requests;
using Tracker.Application.Contracts.Events.Responses;
using Tracker.Application.Interfaces.Events;
using Tracker.Domain;
using Tracker.Domain.Events;
using Tracker.Infrastructure.Persistence;
using Tracker.Infrastructure.Persistence.Specs.Common.Ordering;
using Tracker.Infrastructure.Persistence.Specs.Common.Search;
using Tracker.Infrastructure.Persistence.Specs.Common.Selection;
using Tracker.Infrastructure.Persistence.Specs.Events.Include;
using Tracker.Infrastructure.Persistence.Specs.Events.Projection;
using Tracker.Infrastructure.Persistence.Specs.Events.Search;

namespace Tracker.Infrastructure.Services.Events;

public sealed class EventsService(
    AppDbContext context
) : IEventsService
{
    public async Task<Result<EventSummaryResponse>> CreateAsync(Guid userId, CreateEventRequest request, CancellationToken ctk = default)
    {
        var organizer = await context.Users
            .WithSpecification(new ByIdSpec<User>(userId))
            .FirstOrDefaultAsync(cancellationToken: ctk);

        if (organizer is null)
            return Result.Fail("Organizer not found");

        var draft = Event.CreateDraft(
            request.Title,
            request.StartDate,
            request.EndDate);

        if (draft.IsFailed)
            return draft.ToResult();
        
        var description = draft.Value.UpdateDescription(
            request.Description.Text, 
            request.Description.Format);
        
        if (description.IsFailed)
            return draft.ToResult();
        
        draft.Value.AddOrganizer(
            EventOrganizer.Create(organizer).Value);
        
        if (description.IsFailed)
            return draft.ToResult();

        if (request.Location is not null)
        {
            var location = draft.Value.UpdateLocation(
                request.Location.Name, 
                request.Location.Latitude, 
                request.Location.Longitude);

            if (location.IsFailed)
                return location;
        }
        
        await context.AddAsync(draft.Value, ctk);
        await context.SaveChangesAsync(ctk);

        var groupEvent = draft.Value;
        
        return Result.Ok(new EventSummaryResponse(
            groupEvent.Id,
            groupEvent.Title,
            groupEvent.EndDate,
            groupEvent.StartDate,
            groupEvent.Invitees.Count,
            groupEvent.Organizers.Count));
    }

    public async Task<Result<EventsListResponse>> GetListAsync(Guid organizerId, int offset, int take, CancellationToken ctk = default)
    {
        var groupEvents = await context.Events
            .WithSpecification(new CreatedAtOrderingSpec<Event>())
            .WithSpecification(new SelectionSpec<Event>(offset, take))
            .WithSpecification(new EventByOrganizerIdSpec(organizerId))
            .WithSpecification(new EventToSummarySpec())
            .ToListAsync(cancellationToken: ctk);

        return Result.Ok(new EventsListResponse(
            groupEvents, 
            await context.Events
                .WithSpecification(new EventByOrganizerIdSpec(organizerId))
                .CountAsync(ctk)));
    }

    public async Task<Result<EventDetailsResponse>> GetAsync(Guid organizerId, CancellationToken ctk = default)
    {
        var projection = new ByIdSpec<Event>(organizerId)
            .WithProjectionOf(new EventToDetailsSpec());
        
        var @event = await context.Events
            .WithSpecification(projection)
            .FirstOrDefaultAsync(cancellationToken: ctk);

        if (@event is null)
            return Result.Fail("Event not found");
        
        return Result.Ok(@event);
    }

    public async Task<Result> UpdateTitleAsync(Guid eventId, UpdateEventTitleRequest request, CancellationToken ctk = default)
    {
        var @event = await context.Events
            .WithSpecification(new ByIdSpec<Event>(eventId))
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (@event is null)
            return Result.Fail("Event not found");

        var titleUpdate = @event.UpdateTitle(request.NewTitle);

        if (titleUpdate.IsFailed)
            return titleUpdate;
        
        await context.SaveChangesAsync(ctk);
        
        return Result.Ok();
    }

    public async Task<Result> UpdateDescriptionAsync(Guid eventId, UpdateEventDescriptionRequest request, CancellationToken ctk = default)
    {
        var groupEvent = await context.Events
            .WithSpecification(new ByIdSpec<Event>(eventId))
            .Include(x => x.Description)
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (groupEvent is null)
            return Result.Fail("Event not found");

        var descUpdate = groupEvent.UpdateDescription(
            request.NewDescription.Text,
            request.NewDescription.Format);

        if (descUpdate.IsFailed)
            return descUpdate;
        
        await context.SaveChangesAsync(ctk);
        
        return Result.Ok();
    }

    public async Task<Result> UpdateDatesAsync(Guid eventId, UpdateEventDatesRequest request, CancellationToken ctk = default)
    {
        var groupEvent = await context.Events
            .WithSpecification(new ByIdSpec<Event>(eventId))
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (groupEvent is null)
            return Result.Fail("Event not found");

        var datesUpdate = groupEvent.UpdateDates(
            request.NewStartDate,
            request.NewEndDate);

        if (datesUpdate.IsFailed)
            return datesUpdate;
        
        await context.SaveChangesAsync(ctk);
        
        return Result.Ok();
    }

    public async Task<Result> UpdateLocationAsync(Guid eventId, UpdateEventLocationRequest request, CancellationToken ctk = default)
    {
        var @event = await context.Events
            .WithSpecification(new ByIdSpec<Event>(eventId))
            .WithSpecification(new EventWithLocationSpec())
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (@event is null)
            return Result.Fail("Event not found");

        if (request.NewLocation is { } location)
        {
            var locationUpdate = @event.UpdateLocation(
                location.Name,
                location.Latitude,
                location.Longitude);

            if (locationUpdate.IsFailed)
                return locationUpdate;
        }
        else
        {
            @event.RemoveLocation();
        }
        
        await context.SaveChangesAsync(ctk);
        
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(Guid eventId, CancellationToken ctk = default)
    {
        var rows = await context.Events
            .WithSpecification(new ByIdSpec<Event>(eventId))
            .ExecuteDeleteAsync(cancellationToken: ctk);
        
        return Result.FailIf(rows <= 0, "Failed to delete event");
    }

    public Task<Result> AttachToGroupAsync(Guid eventId, Guid groupId, CancellationToken ctk = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DetachFromGroupAsync(Guid eventId, Guid groupId, CancellationToken ctk = default)
    {
        throw new NotImplementedException();
    }
}