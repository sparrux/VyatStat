namespace Tracker.Application.Contracts.Organizers.Requests;

public sealed record CreateEventOrganizerRequest(
    Guid UserId
);