namespace Hub.Application.Features.Events.Contracts;

public sealed record EventSummaryResponse(
    Guid Id,
    string Title,
    DateTimeOffset EndDate,
    DateTimeOffset StartDate,
    int InviteesCount,
    int OrganizersCount
);