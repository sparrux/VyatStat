using Tracker.Application.Contracts.Users.Responses;

namespace Tracker.Application.Contracts.Invitees.Responses;

public sealed record EventInviteeSummaryResponse(
    Guid Id,
    UserSummaryResponse User
);