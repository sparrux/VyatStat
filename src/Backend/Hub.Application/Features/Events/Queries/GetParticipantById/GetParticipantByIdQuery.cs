namespace Hub.Application.Features.Events.Queries.GetParticipantById;

public sealed record GetParticipantByIdQuery(
    Guid EventId,
    Guid ParticipantUserId
);
