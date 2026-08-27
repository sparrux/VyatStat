namespace Hub.Application.Features.Events.Contracts;

public sealed record EventParticipantsListResponse(
    IReadOnlyCollection<EventParticipantSummaryResponse> Participants,
    int TotalCount
);