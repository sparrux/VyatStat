using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tracker.Application.Contracts.GroupEvents.Requests;
using Tracker.Application.Contracts.GroupEvents.Responses;
using Tracker.Application.Services.GroupEvents;
using Tracker.Domain;
using Tracker.Domain.GroupEvents;
using Tracker.Domain.Groups;
using Tracker.Infrastructure.Persistence;
using Tracker.Infrastructure.Persistence.Specs.Common;
using Tracker.Infrastructure.Persistence.Specs.GroupEvents;

namespace Tracker.Infrastructure.Services.GroupEvents;

public sealed class GroupEventsService(
    AppDbContext context
) : IGroupEventsService
{
    public async Task<Result<GroupEventSummaryResponse>> CreateAsync(Guid groupId, Guid orgId, CreateGroupEventRequest request, CancellationToken ctk = default)
    {
        var group = await context.Groups
            .WithSpecification(new ByIdSpec<Group>(groupId))
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (group is null)
            return Result.Fail("Group not found");
        
        var organizer = await context.Users
            .WithSpecification(new ByIdSpec<User>(orgId))
            .FirstOrDefaultAsync(cancellationToken: ctk);

        if (organizer is null)
            return Result.Fail("Organizer not found");

        var draft = group.AddEvent(
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
            GroupEventOrganizer.Create(organizer).Value);
        
        if (description.IsFailed)
            return draft.ToResult();

        if (request.Location is not null)
        {
            var location = draft.Value.UpdateLocation(
                Location.Create(
                    request.Location.Name, 
                    request.Location.Latitude, 
                    request.Location.Longitude).Value);

            if (location.IsFailed)
                return location;
        }
        
        await context.AddAsync(draft.Value, ctk);
        await context.SaveChangesAsync(ctk);

        var groupEvent = draft.Value;
        
        return Result.Ok(new GroupEventSummaryResponse(
            groupEvent.Id,
            groupEvent.Title,
            groupEvent.EndDate,
            groupEvent.StartDate,
            groupEvent.Invitees.Count,
            groupEvent.Organizers.Count));
    }

    public async Task<Result<GroupEventsListResponse>> GetListAsync(Guid groupId, int offset, int take, CancellationToken ctk = default)
    {
        var ordering = new CreatedAtOrderingSpec<GroupEvent>();
        var selection = new SelectionSpec<GroupEvent>(offset, take);
        
        var projection = new ByGroupIdSpec(groupId)
            .WithProjectionOf(new GroupEventToSummarySpec());
        
        var groupEvents = await context.GroupEvents
            .WithSpecification(ordering)
            .WithSpecification(selection)
            .WithSpecification(projection)
            .ToListAsync(cancellationToken: ctk);

        return Result.Ok(new GroupEventsListResponse(
            groupEvents, 
            await context.GroupEvents
                .WithSpecification(new ByGroupIdSpec(groupId))
                .CountAsync(ctk)));
    }

    public async Task<Result<GroupEventDetailsResponse>> GetAsync(Guid eventId, CancellationToken ctk = default)
    {
        var projection = new ByIdSpec<GroupEvent>(eventId)
            .WithProjectionOf(new GroupEventToDetailsSpec());
        
        var groupEvent = await context.GroupEvents
            .WithSpecification(projection)
            .FirstOrDefaultAsync(cancellationToken: ctk);

        if (groupEvent is null)
            return Result.Fail("Group event not found");
        
        return Result.Ok(groupEvent);
    }

    public async Task<Result> UpdateTitleAsync(Guid eventId, UpdateGroupEventTitleRequest request, CancellationToken ctk = default)
    {
        var groupEvent = await context.GroupEvents
            .WithSpecification(new ByIdSpec<GroupEvent>(eventId))
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (groupEvent is null)
            return Result.Fail("Group event not found");

        var titleUpdate = groupEvent.UpdateTitle(request.NewTitle);

        if (titleUpdate.IsFailed)
            return titleUpdate;
        
        await context.SaveChangesAsync(ctk);
        
        return Result.Ok();
    }

    public async Task<Result> UpdateDescriptionAsync(Guid eventId, UpdateGroupEventDescriptionRequest request, CancellationToken ctk = default)
    {
        var groupEvent = await context.GroupEvents
            .WithSpecification(new ByIdSpec<GroupEvent>(eventId))
            .Include(x => x.Description)
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (groupEvent is null)
            return Result.Fail("Group event not found");

        var descUpdate = groupEvent.UpdateDescription(
            request.NewDescription.Text,
            request.NewDescription.Format);

        if (descUpdate.IsFailed)
            return descUpdate;
        
        await context.SaveChangesAsync(ctk);
        
        return Result.Ok();
    }

    public async Task<Result> UpdateDatesAsync(Guid eventId, UpdateGroupEventDatesRequest request, CancellationToken ctk = default)
    {
        var groupEvent = await context.GroupEvents
            .WithSpecification(new ByIdSpec<GroupEvent>(eventId))
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (groupEvent is null)
            return Result.Fail("Group event not found");

        var datesUpdate = groupEvent.UpdateDates(
            request.NewStartDate,
            request.NewEndDate);

        if (datesUpdate.IsFailed)
            return datesUpdate;
        
        await context.SaveChangesAsync(ctk);
        
        return Result.Ok();
    }

    public async Task<Result> UpdateLocationAsync(Guid eventId, UpdateGroupEventLocationRequest request, CancellationToken ctk = default)
    {
        var groupEvent = await context.GroupEvents
            .WithSpecification(new ByIdSpec<GroupEvent>(eventId))
            .WithSpecification(new WithLocationSpec())
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (groupEvent is null)
            return Result.Fail("Group event not found");

        var newLocationReq = request.NewLocation;
        var newLocation = newLocationReq is null 
            ? null
            : Location.Create(
                newLocationReq.Name,
                newLocationReq.Latitude,
                newLocationReq.Longitude);
        
        if (newLocation?.IsFailed ?? false)
            return newLocation.ToResult();

        var locationUpdate = groupEvent.UpdateLocation(newLocation?.Value);

        if (locationUpdate.IsFailed)
            return locationUpdate;
        
        await context.SaveChangesAsync(ctk);
        
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(Guid eventId, CancellationToken ctk = default)
    {
        var rows = await context.GroupEvents
            .WithSpecification(new ByIdSpec<GroupEvent>(eventId))
            .ExecuteDeleteAsync(cancellationToken: ctk);
        
        return Result.FailIf(rows <= 0, "Failed to delete group event");
    }
}