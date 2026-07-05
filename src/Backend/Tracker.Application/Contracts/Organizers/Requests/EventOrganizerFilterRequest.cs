namespace Tracker.Application.Contracts.Organizers.Requests;

public sealed record EventOrganizerFilterRequest(
    Guid? OrganizerUserId,
    Guid? EventId
);