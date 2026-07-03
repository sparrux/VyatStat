using Ardalis.Specification.EntityFrameworkCore;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tracker.Application.Contracts.Event.Responses;
using Tracker.Application.Contracts.User.Responses;
using Tracker.Application.Services.Invitees;
using Tracker.Application.Services.Requirements;
using Tracker.Domain;
using Tracker.Domain.GroupEvents.Events;
using Tracker.Infrastructure.Persistence;
using Tracker.Infrastructure.Persistence.Specs.Common;
using Tracker.Infrastructure.Persistence.Specs.GroupEvents;
using Tracker.Infrastructure.Persistence.Specs.Invitees;

namespace Tracker.Infrastructure.Services.Invitees;

public sealed class InviteesService(
    AppDbContext context,
    IRequirementsSynchronization synchronization
) : IInviteesService
{
    public async Task<Result<GroupEventInviteeSummaryResponse>> CreateAsync(Guid eventId, Guid userId, CancellationToken ctk = default)
    {
        var groupEvent = await context.GroupEvents
            .WithSpecification(new ByIdSpec<GroupEvent>(eventId))
            .WithSpecification(new WithRequirementsSpec())
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (groupEvent is null)
            return Result.Fail("Group event not found");

        var user = await context.Users
            .WithSpecification(new ByIdSpec<User>(userId))
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (user is null)
            return Result.Fail("User not found");
        
        if (await context.GroupEventInvitees.AnyAsync(i => 
                i.EventId == eventId && i.UserId == userId, cancellationToken: ctk))
            return Result.Fail("Invitee already exists");

        var addInvitee = groupEvent.AddInvitee(user);

        if (addInvitee.IsFailed)
            return addInvitee.ToResult();

        await context.SaveChangesAsync(ctk);
        await synchronization.SynchronizeAsync(groupEvent, addInvitee.Value, ctk);

        return Result.Ok(
            new GroupEventInviteeSummaryResponse(
                addInvitee.Value.Id,
                new UserSummaryResponse(
                    addInvitee.Value.User.Id,
                    addInvitee.Value.User.Nickname,
                    addInvitee.Value.User.CreatedAt)));
    }

    public async Task<Result<GroupEventInviteesListResponse>> GetListAsync(
        Guid eventId, int offset, int take, CancellationToken ctk = default)
    {
        var invitees = await context.GroupEventInvitees
            .WithSpecification(new ByEventIdSpec(eventId))
            .WithSpecification(new SelectionSpec<GroupEventInvitee>(offset, take))
            .WithSpecification(new InviteeToSummarySpec())
            .ToListAsync(cancellationToken: ctk);

        return Result.Ok(new GroupEventInviteesListResponse(
            invitees, 
            await context.GroupEventInvitees
                .WithSpecification(new ByEventIdSpec(eventId))
                .CountAsync(ctk)));
    }

    public async Task<Result<GroupEventInviteeDetailsResponse>> GetAsync(Guid eventId, Guid userId, CancellationToken ctk = default)
    {
        var invitee = await context.GroupEventInvitees
            .WithSpecification(new ByEventIdSpec(eventId))
            .WithSpecification(new ByUserIdSpec(userId))
            .WithSpecification(new InviteeToDetailsSpec())
            .FirstOrDefaultAsync(ctk);

        if (invitee is null)
            return Result.Fail("Invitee not found");

        return Result.Ok(invitee);
    }
}