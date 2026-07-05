namespace Tracker.Application.Contracts.Organizers.Responses;

public sealed record EventOrganizersListResponse(
    IReadOnlyCollection<EventOrganizerResponse> Organizers,
    int TotalCount
);