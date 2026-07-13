using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Common.Specifications;
using Hub.Application.Features.Events.Specifications.Include;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.UpdateCompletion;

sealed class UpdateCompletionCommandHandler(
    HubDbContext dbContext
) : IRequestHandler<UpdateCompletionCommand, IdResponse>
{
    public async Task<Result<IdResponse>> Handle(
        UpdateCompletionCommand request, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new EventWithInviteesSpec())
            .WithSpecification(new EventWithRequirementsSpec())
            .WithSpecification(new EventWithRequirementCompletionsSpec())
            .WithSpecification(new GetByIdSpec<Event>(request.EventId))
            .FirstOrDefaultAsync(cancellationToken);

        if (ev is null) return Result.NotFound("Event not found by id");

        var updateResult = ev.UpdateCompletionStatus(
            request.UserId,
            request.RequirementId,
            request.NewStatus);

        if (!updateResult.IsSuccess) return updateResult.Map();
        
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(new IdResponse(ev.Id));
    }
}