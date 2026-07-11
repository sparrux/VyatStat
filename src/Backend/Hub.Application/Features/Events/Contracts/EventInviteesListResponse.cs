namespace Hub.Application.Features.Events.Contracts;

public sealed record EventInviteesListResponse(
    IReadOnlyCollection<EventInviteeSummaryResponse> Invitees,
    int TotalCount
);