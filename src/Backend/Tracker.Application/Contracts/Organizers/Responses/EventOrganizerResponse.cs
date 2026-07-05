using Tracker.Application.Contracts.Users.Responses;

namespace Tracker.Application.Contracts.Organizers.Responses;

public sealed record EventOrganizerResponse(
    Guid Id,
    UserSummaryResponse User
);