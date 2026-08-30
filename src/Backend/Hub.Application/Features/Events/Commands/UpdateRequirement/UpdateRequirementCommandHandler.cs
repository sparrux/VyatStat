using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Common.Specifications.Search;
using Hub.Application.Features.Events.Specifications.Include;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.UpdateRequirement;

sealed class UpdateRequirementCommandHandler(
    IHubDbContext dbContext
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

        var requirement = ev.Requirements.FirstOrDefault(x => x.Id == command.RequirementId);
        if (requirement is null) return Result.NotFound("Requirement not found by id");

        var updateResult = ev.UpdateRequirement(
            requirement,
            command.Request.Title,
            command.Request.Description);

        if (!updateResult.IsSuccess) return updateResult.Map();

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new IdResponse(ev.Id));
    }
}
