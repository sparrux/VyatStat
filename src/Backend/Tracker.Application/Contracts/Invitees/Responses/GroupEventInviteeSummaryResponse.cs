using Tracker.Application.Contracts.Users.Responses;

namespace Tracker.Application.Contracts.Invitees.Responses;

public sealed record GroupEventInviteeSummaryResponse(
    Guid Id,
    UserSummaryResponse User
);