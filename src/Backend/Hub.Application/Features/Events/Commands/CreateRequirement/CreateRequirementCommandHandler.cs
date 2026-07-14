using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Specifications;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Events.Specifications.Include;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.CreateRequirement;

sealed class CreateRequirementCommandHandler(
    HubDbContext dbContext
) : IRequestHandler<CreateRequirementCommand, EventRequirementSummaryResponse>
{
    public async Task<Result<EventRequirementSummaryResponse>> Handle(
        CreateRequirementCommand command, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new EventWithRequirementsSpec())
            .WithSpecification(new GetByIdSpec<Event>(command.EventId))
            .FirstOrDefaultAsync(cancellationToken);

        if (ev is null) return Result.NotFound("Event not found by id");

        var request = command.Request;

        var addRequirement = ev.AddRequirement(
            request.Title, 
            request.Description, 
            request.IsMandatory, 
            request.VerificationMode);

        if (!addRequirement.IsSuccess) return addRequirement.Map();

        await dbContext.SaveChangesAsync(cancellationToken);

        var requirement = addRequirement.Value;

        return Result.Success(new EventRequirementSummaryResponse(
            requirement.Id,
            requirement.Title,
            requirement.Description,
            requirement.IsMandatory,
            requirement.VerificationMode));
    }
}