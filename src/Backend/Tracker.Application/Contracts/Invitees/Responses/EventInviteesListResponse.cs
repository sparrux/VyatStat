namespace Tracker.Application.Contracts.Invitees.Responses;

public sealed record EventInviteesListResponse(
    IReadOnlyCollection<EventInviteeSummaryResponse> Invitees,
    int TotalCount
);