using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Contracts;

public sealed record EventSummaryResponse(
    Guid Id,
    string Title,
    EventState State,
    DateTimeOffset EndDate,
    DateTimeOffset StartDate,
    bool HasLocation,
    int InviteesCount,
    int OrganizersCount,
    int RequirementsCount,
    int GoalsCount
);