using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Specifications.Search;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Events.Specifications.Include;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.CreateRequirement;

sealed class CreateRequirementCommandHandler(
    IHubDbContext dbContext
) : IRequestHandler<CreateRequirementCommand, EventRequirementSummaryResponse>
{
    public async Task<Result<EventRequirementSummaryResponse>> Handle(
        CreateRequirementCommand command, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new EventWithRequirementsSpec())
            .WithSpecification(new EventWithRequirementAssignmentsSpec())
            .WithSpecification(new GetByIdSpec<Event>(command.EventId))
            .FirstOrDefaultAsync(cancellationToken);

        if (ev is null) return Result.NotFound("Event not found by id");

        var request = command.Request;
        var addResult = ev.AddRequirement(
            request.Title,
            request.Description,
            request.AssignmentPolicy);

        if (!addResult.IsSuccess) return addResult.Map();

        await dbContext.SaveChangesAsync(cancellationToken);

        var requirement = addResult.Value;
        return Result.Created(new EventRequirementSummaryResponse(
            requirement.Id,
            requirement.Title,
            requirement.Description));
    }
}
