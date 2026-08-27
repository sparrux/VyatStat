using Hub.Application.Features.Users.Contracts;

namespace Hub.Application.Features.Events.Contracts;

public sealed record EventParticipantSummaryResponse(
    Guid Id,
    UserSummaryResponse User
);