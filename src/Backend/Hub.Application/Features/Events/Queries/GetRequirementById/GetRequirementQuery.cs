namespace Hub.Application.Features.Events.Queries.GetRequirementById;

public sealed record GetRequirementQuery(
    Guid EventId,
    Guid RequirementId
);