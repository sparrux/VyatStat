using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Specifications.Search;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Events.Specifications.Include;
using Hub.Application.Features.Events.Specifications.Search;
using Hub.Application.Pipelines;
using Hub.Domain.Events.Requirements;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Queries.GetRequirementById;

sealed class GetRequirementByIdQueryHandler(
    HubDbContext context
) : IRequestHandler<GetRequirementQuery, EventRequirementDetailsResponse>
{
    public async Task<Result<EventRequirementDetailsResponse>> Handle(
        GetRequirementQuery query, CancellationToken cancellationToken)
    {
        var requirement = await context.EventRequirements
            .WithSpecification(new GetByIdSpec<EventRequirement>(query.RequirementId))
            .WithSpecification(new GetRequirementByEventIdSpec(query.EventId))
            .WithSpecification(new RequirementWithVerifiersSpec())
            .FirstOrDefaultAsync(cancellationToken);

        if (requirement is null) return Result.NotFound("Requirement not found");

        return Result.Success(new EventRequirementDetailsResponse(
            requirement.Id,
            requirement.EventId,
            requirement.Title,
            requirement.Description,
            requirement.AssignmentPolicy,
            requirement.Verifiers
                .OrderBy(v => v.CreatedAt)
                .Select(v => v.ToResponse())
                .ToList()));
    }
}
