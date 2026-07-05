namespace Tracker.Application.Contracts.Events.Responses;

public sealed record EventSummaryResponse(
    Guid Id,
    string Title,
    DateTimeOffset EndDate,
    DateTimeOffset StartDate,
    int InviteesCount,
    int OrganizersCount
);