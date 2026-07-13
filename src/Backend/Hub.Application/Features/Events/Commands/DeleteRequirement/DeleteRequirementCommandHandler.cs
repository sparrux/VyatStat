using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Common.Specifications;
using Hub.Application.Features.Events.Specifications.Include;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.DeleteRequirement;

sealed class DeleteRequirementCommandHandler(
    HubDbContext dbContext
) : IRequestHandler<DeleteRequirementCommand, IdResponse>
{
    public async Task<Result<IdResponse>> Handle(
        DeleteRequirementCommand command, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new EventWithRequirementsSpec())
            .WithSpecification(new EventWithRequirementCompletionsSpec())
            .WithSpecification(new GetByIdSpec<Event>(command.EventId))
            .FirstOrDefaultAsync(cancellationToken);

        if (ev is null) return Result.NotFound("Event not found by id");

        var requirement = ev.Requirements.FirstOrDefault(
            x => x.Id == command.RequirementId);
        
        if (requirement is null) return Result.NotFound("Requirement not found by id");
        
        var removeResult = ev.RemoveRequirement(requirement);
        if (!removeResult.IsSuccess) return removeResult.Map();
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new IdResponse(ev.Id));
    }
}