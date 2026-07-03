namespace Tracker.Application.Contracts.Event.Responses;

public sealed record GroupEventInviteesListResponse(
    IReadOnlyCollection<GroupEventInviteeSummaryResponse> Invitees,
    int TotalCount
);