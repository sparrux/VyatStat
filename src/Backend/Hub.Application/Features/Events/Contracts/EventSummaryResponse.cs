using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Contracts;

public sealed record EventSummaryResponse(
    Guid Id,
    string Title,
    EventState State,
    DateTimeOffset EndDate,
    DateTimeOffset StartDate,
    bool HasLocation,
    int ParticipantsCount,
    int RequirementsCount,
    int GoalsCount,
    int RolesCount
);