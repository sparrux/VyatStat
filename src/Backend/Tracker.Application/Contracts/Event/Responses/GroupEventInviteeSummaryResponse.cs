using Tracker.Application.Contracts.User.Responses;

namespace Tracker.Application.Contracts.Event.Responses;

public sealed record GroupEventInviteeSummaryResponse(
    Guid Id,
    UserSummaryResponse User
);