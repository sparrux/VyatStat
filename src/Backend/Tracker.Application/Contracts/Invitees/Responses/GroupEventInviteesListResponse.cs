namespace Tracker.Application.Contracts.Invitees.Responses;

public sealed record GroupEventInviteesListResponse(
    IReadOnlyCollection<GroupEventInviteeSummaryResponse> Invitees,
    int TotalCount
);