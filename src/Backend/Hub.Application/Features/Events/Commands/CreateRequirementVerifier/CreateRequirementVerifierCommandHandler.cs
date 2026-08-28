using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Specifications.Search;
using Hub.Application.Features.Events.Commands.CreateRequirementVerifier.Appliers;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Events.Specifications.Include;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.CreateRequirementVerifier;

sealed class CreateRequirementVerifierCommandHandler(
    HubDbContext context,
    IEnumerable<IRequirementVerifierApplier> appliers
) : IRequestHandler<CreateRequirementVerifierCommand, EventRequirementVerifierSummaryResponse>
{
    public async Task<Result<EventRequirementVerifierSummaryResponse>> Handle(
        CreateRequirementVerifierCommand command, CancellationToken cancellationToken)
    {
        var evt = await context.Events
            .WithSpecification(new GetByIdSpec<Event>(command.EventId))
            .WithSpecification(new EventWithRequirementVerifiersSpec())
            .FirstOrDefaultAsync(cancellationToken);

        if (evt is null) return Result.NotFound("Event not found");
        
        var requirement = evt.Requirements.FirstOrDefault(x => x.Id == command.RequirementId);

        if (requirement is null) return Result.NotFound("Requirement not found");

        var applier = appliers.Single(x => x.CanApply(command.Request));
        
        var verifier = await applier.ApplyAsync(
            new ApplyContext(evt, requirement), command.Request, cancellationToken);

        if (!verifier.IsSuccess) return verifier.Map();

        await context.AddAsync(verifier.Value, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Created(new EventRequirementVerifierSummaryResponse(
            verifier.Value.Id,
            verifier.Value.IsRequired,
            verifier.Value.DetectResponseType()));
    }
}