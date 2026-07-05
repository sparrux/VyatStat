using Ardalis.Specification.EntityFrameworkCore;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tracker.Application.Contracts.Requirements.Requests;
using Tracker.Application.Contracts.Requirements.Responses;
using Tracker.Application.Interfaces.Requirements;
using Tracker.Domain.Events;
using Tracker.Domain.Events.Requirements;
using Tracker.Infrastructure.Persistence;
using Tracker.Infrastructure.Persistence.Specs.Common.Search;
using Tracker.Infrastructure.Persistence.Specs.Events.Include;
using Tracker.Infrastructure.Persistence.Specs.Events.Search;

namespace Tracker.Infrastructure.Services.Requirements;

public sealed class RequirementsService(
    AppDbContext context,
    IRequirementsSynchronization synchronization
) : IRequirementsService
{
    public async Task<Result<EventRequirementResponse>> CreateAsync(Guid eventId, CreateEventRequirementRequest request, CancellationToken ctk = default)
    {
        var groupEvent = await context.Events
            .WithSpecification(new ByIdSpec<Event>(eventId))
            .WithSpecification(new EventWithRequirementsSpec())
            .WithSpecification(new EventWithRequirementsCompletionsSpec())
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (groupEvent is null)
            return Result.Fail("Group event not found");

        var requirement = EventRequirement
            .Create(request.Title, request.Description, request.IsMandatory, request.ConfirmationMode);
        
        if (requirement.IsFailed)
            return requirement.ToResult();
        
        var addRequirement = groupEvent.AddRequirement(requirement.Value);
        
        if (addRequirement.IsFailed)
            return addRequirement;
        
        await context.SaveChangesAsync(ctk);
        await synchronization.SynchronizeAsync(groupEvent, ctk);

        return Result.Ok(new EventRequirementResponse(
            requirement.Value.Id,
            requirement.Value.Title,
            requirement.Value.Description,
            requirement.Value.IsMandatory,
            requirement.Value.ConfirmationMode));
    }

    public async Task<Result<EventRequirementResponse>> UpdateAsync(Guid eventId, Guid reqId, UpdateEventRequirementRequest request, CancellationToken ctk = default)
    {
        var groupEvent = await context.Events
            .WithSpecification(new ByIdSpec<Event>(eventId))
            .WithSpecification(new EventWithRequirementsSpec())
            .FirstOrDefaultAsync(cancellationToken: ctk);
        
        if (groupEvent is null)
            return Result.Fail("Group event not found");
        
        var updateRequirement = groupEvent.UpdateRequirement(
            reqId, request.Title, request.Description, request.IsMandatory, request.ConfirmationMode);
        
        if (updateRequirement.IsFailed)
            return updateRequirement;
        
        await context.SaveChangesAsync(ctk);
        
        var requirement = groupEvent.Requirements.First(x => x.Id == reqId);

        return Result.Ok(new EventRequirementResponse(
            requirement.Id,
            requirement.Title,
            requirement.Description,
            requirement.IsMandatory,
            requirement.ConfirmationMode));
    }

    public async Task<Result> DeleteAsync(Guid eventId, Guid reqId, CancellationToken ctk = default)
    {
        var groupEvent = await context.Events
            .WithSpecification(new ByIdSpec<Event>(eventId))
            .WithSpecification(new EventByRequirementIdSpec(reqId))
            .WithSpecification(new EventWithRequirementsSpec())
            .WithSpecification(new EventWithRequirementsCompletionsSpec())
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