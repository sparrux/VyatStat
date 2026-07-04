using Tracker.Application.Contracts.Users.Responses;

namespace Tracker.Application.Contracts.Organizers.Responses;

public sealed record GroupEventOrganizerResponse(
    UserSummaryResponse User
);