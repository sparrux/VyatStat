namespace Tracker.Application.Contracts.Event.Responses;

public sealed record GroupEventSummaryResponse(
    Guid Id,
    string Title,
    DateTimeOffset EndDate,
    DateTimeOffset StartDate,
    int InviteesCount,
    int OrganizersCount
);