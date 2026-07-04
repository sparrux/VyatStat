using Ardalis.Specification.EntityFrameworkCore;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tracker.Application.Contracts.Requirements.Requests;
using Tracker.Application.Contracts.Requirements.Responses;
using Tracker.Application.Services.Requirements;
using Tracker.Domain.GroupEvents;
using Tracker.Infrastructure.Persistence;
using Tracker.Infrastructure.Persistence.Specs.Common;
using Tracker.Infrastructure.Persistence.Specs.GroupEvents;

namespace Tracker.Infrastructure.Services.Requirements;

public sealed class RequirementsService(
    AppDbContext context,
    IRequirementsSynchronization synchronization
) : IRequirementsService
{
    public async Task<Result<GroupEventRequirementResponse>> CreateAsync(Guid eventId, CreateGroupEventRequirementRequest request, CancellationToken ctk = default)
    {
        var groupEvent = await context.GroupEvents
            .WithSpecification(new ByIdSpec<GroupEvent>(eventId))
            .WithSpecification(new WithRequirementsSpec())
            .WithSpecification(new WithInviteesCompletionsSpec())
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (groupEvent is null)
            return Result.Fail("Group event not found");

        var maxSortOrder = groupEvent.Requirements.Count > 0
            ? groupEvent.Requirements.Max(x => x.SortOrder)
            : 0;

        var requirement = GroupEventRequirement
            .Create(request.Title, request.Description, request.IsMandatory, maxSortOrder + 1);
        
        if (requirement.IsFailed)
            return requirement.ToResult();
        
        var addRequirement = groupEvent.AddRequirement(requirement.Value);
        
        if (addRequirement.IsFailed)
            return addRequirement;
        
        await context.SaveChangesAsync(ctk);
        await synchronization.SynchronizeAsync(groupEvent, ctk);

        return Result.Ok(new GroupEventRequirementResponse(
            requirement.Value.Id,
            requirement.Value.Title,
            requirement.Value.Description,
            requirement.Value.IsMandatory));
    }

    public async Task<Result<GroupEventRequirementResponse>> UpdateAsync(Guid eventId, Guid reqId, UpdateGroupEventRequirementRequest request, CancellationToken ctk = default)
    {
        var groupEvent = await context.GroupEvents
            .WithSpecification(new ByIdSpec<GroupEvent>(eventId))
            .WithSpecification(new WithRequirementsSpec())
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (groupEvent is null)
            return Result.Fail("Group event not found");
        
        var updateRequirement = groupEvent.UpdateRequirement(
            reqId, request.Title, request.Description, request.IsMandatory);
        
        if (updateRequirement.IsFailed)
            return updateRequirement;
        
        await context.SaveChangesAsync(ctk);
        
        var requirement = groupEvent.Requirements.First(x => x.Id == reqId);

        return Result.Ok(new GroupEventRequirementResponse(
            requirement.Id,
            requirement.Title,
            requirement.Description,
            requirement.IsMandatory));
    }

    public async Task<Result> DeleteAsync(Guid eventId, Guid reqId, CancellationToken ctk = default)
    {
        var groupEvent = await context.GroupEvents
            .WithSpecification(new ByIdSpec<GroupEvent>(eventId))
            .WithSpecification(new ByRequirementIdSpec(reqId))
            .WithSpecification(new WithRequirementsSpec())
            .WithSpecification(new WithInviteesCompletionsSpec())
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (groupEvent is null)
            return Result.Fail("Group event or requirement not found");
        
        var requirement = groupEvent.Requirements.First(x => x.Id == reqId);
        
        var removeRequirement = groupEvent.RemoveRequirement(requirement);

        if (removeRequirement.IsFailed)
            return removeRequirement;

        await context.SaveChangesAsync(ctk);
        await synchronization.SynchronizeAsync(groupEvent, ctk);

        return Result.Ok();
    }
}