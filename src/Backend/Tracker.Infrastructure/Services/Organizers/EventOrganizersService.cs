using Ardalis.Specification.EntityFrameworkCore;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tracker.Application.Contracts.Common.Requests;
using Tracker.Application.Contracts.Organizers.Requests;
using Tracker.Application.Contracts.Organizers.Responses;
using Tracker.Application.Contracts.Users.Responses;
using Tracker.Application.Interfaces.Organizers;
using Tracker.Domain;
using Tracker.Domain.Events;
using Tracker.Infrastructure.Persistence;
using Tracker.Infrastructure.Persistence.Specs.Common.Search;

namespace Tracker.Infrastructure.Services.Organizers;

public sealed class EventOrganizersService(AppDbContext context) : IEventOrganizersService
{
    public async Task<Result<EventOrganizerResponse>> CreateAsync(
        Guid eventId, CreateEventOrganizerRequest request, CancellationToken ctk = default)
    {
        var @event = await context.Events
            .WithSpecification(new ByIdSpec<Event>(eventId))
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (@event is null)
            return Result.Fail("Event not found");
        
        var user = await context.Users
            .WithSpecification(new ByIdSpec<User>(request.UserId))
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (user is null)
            return Result.Fail("User not found");

        var contains = await context.EventOrganizers
            .Where(x => x.UserId == request.UserId)
            .AnyAsync(cancellationToken: ctk);
        
        if (contains)
            return Result.Fail("User already organizer");

        var result = @event.AddOrganizer(user);

        if (result.IsFailed)
            return result.ToResult();
        
        await context.AddAsync(result.Value, ctk);
        await context.SaveChangesAsync(ctk);
        
        return new EventOrganizerResponse(
            result.Value.Id,
            new UserSummaryResponse(
                user.Id,
                user.Nickname,
                user.CreatedAt));
    }

    public Task<Result<EventOrganizerResponse>> GetListAsync(
        EventOrganizerFilterRequest request, ListSelectionRequest selection, CancellationToken ctk = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<EventOrganizerResponse>> GetAsync(Guid organizerId, CancellationToken ctk = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteAsync(Guid organizerId, CancellationToken ctk = default)
    {
        throw new NotImplementedException();
    }
}