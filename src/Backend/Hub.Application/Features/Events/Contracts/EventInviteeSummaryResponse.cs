using Hub.Application.Features.Users.Contracts;

namespace Hub.Application.Features.Events.Contracts;

public sealed record EventInviteeSummaryResponse(
    Guid Id,
    UserSummaryResponse User
);