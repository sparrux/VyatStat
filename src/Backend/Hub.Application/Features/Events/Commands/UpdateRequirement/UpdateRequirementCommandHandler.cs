using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Common.Specifications;
using Hub.Application.Features.Events.Specifications.Include;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.UpdateRequirement;

sealed class UpdateRequirementCommandHandler(
    HubDbContext dbContext
) : IRequestHandler<UpdateRequirementCommand, IdResponse>
{
    public async Task<Result<IdResponse>> Handle(
        UpdateRequirementCommand command, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new EventWithRequirementsSpec())
            .WithSpecification(new GetByIdSpec<Event>(command.EventId))
            .FirstOrDefaultAsync(cancellationToken);

        if (ev is null) return Result.NotFound("Event not found by id");

        var request = command.Request;

        var updateRequirement = ev.UpdateRequirement(
            command.RequirementId,
            request.Title,
            request.Description,
            request.IsMandatory,
            request.VerificationMode);

        if (!updateRequirement.IsSuccess) return updateRequirement.Map();
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success(new IdResponse(command.EventId));
    }
}