using Ardalis.Specification.EntityFrameworkCore;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tracker.Application.Contracts.Invitees.Responses;
using Tracker.Application.Contracts.Users.Responses;
using Tracker.Application.Interfaces.Invitees;
using Tracker.Application.Interfaces.Requirements;
using Tracker.Domain;
using Tracker.Domain.Events;
using Tracker.Domain.Events.Invitees;
using Tracker.Infrastructure.Persistence;
using Tracker.Infrastructure.Persistence.Specs.Common.Search;
using Tracker.Infrastructure.Persistence.Specs.Common.Selection;
using Tracker.Infrastructure.Persistence.Specs.Events.Include;
using Tracker.Infrastructure.Persistence.Specs.Invitees.Projection;
using Tracker.Infrastructure.Persistence.Specs.Invitees.Search;

namespace Tracker.Infrastructure.Services.Invitees;

public sealed class InviteesService(
    AppDbContext context,
    IRequirementsSynchronization synchronization
) : IInviteesService
{
    public async Task<Result<EventInviteeSummaryResponse>> CreateAsync(Guid eventId, Guid userId, CancellationToken ctk = default)
    {
        var groupEvent = await context.Events
            .WithSpecification(new ByIdSpec<Event>(eventId))
            .WithSpecification(new EventWithRequirementsSpec())
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (groupEvent is null)
            return Result.Fail("Group event not found");

        var user = await context.Users
            .WithSpecification(new ByIdSpec<User>(userId))
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (user is null)
            return Result.Fail("User not found");
        
        if (await context.EventInvitees.AnyAsync(i => 
                i.EventId == eventId && i.UserId == userId, cancellationToken: ctk))
            return Result.Fail("Invitee already exists");

        var addInvitee = groupEvent.AddInvitee(user);

        if (addInvitee.IsFailed)
            return addInvitee.ToResult();

        await context.SaveChangesAsync(ctk);
        await synchronization.SynchronizeAsync(groupEvent, addInvitee.Value, ctk);

        return Result.Ok(
            new EventInviteeSummaryResponse(
                addInvitee.Value.Id,
                new UserSummaryResponse(
                    addInvitee.Value.User.Id,
                    addInvitee.Value.User.Nickname,
                    addInvitee.Value.User.CreatedAt)));
    }

    public async Task<Result<EventInviteesListResponse>> GetListAsync(
        Guid eventId, int offset, int take, CancellationToken ctk = default)
    {
        var invitees = await context.EventInvitees
            .WithSpecification(new InviteeByEventIdSpec(eventId))
            .WithSpecification(new SelectionSpec<EventInvitee>(offset, take))
            .WithSpecification(new InviteeToSummarySpec())
            .ToListAsync(cancellationToken: ctk);

        return Result.Ok(new EventInviteesListResponse(
            invitees, 
            await context.EventInvitees
                .WithSpecification(new InviteeByEventIdSpec(eventId))
                .CountAsync(ctk)));
    }

    public async Task<Result<EventInviteeDetailsResponse>> GetAsync(Guid eventId, Guid userId, CancellationToken ctk = default)
    {
        var invitee = await context.EventInvitees
            .WithSpecification(new InviteeByEventIdSpec(eventId))
            .WithSpecification(new InviteeByUserIdSpec(userId))
            .WithSpecification(new InviteeToDetailsSpec())
            .FirstOrDefaultAsync(ctk);

        if (invitee is null)
            return Result.Fail("Invitee not found");

        return Result.Ok(invitee);
    }
}