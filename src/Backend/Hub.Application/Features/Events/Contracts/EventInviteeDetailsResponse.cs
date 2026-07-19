using Hub.Application.Features.Users.Contracts;
using Hub.Domain.Events.Participants;

namespace Hub.Application.Features.Events.Contracts;

public sealed record EventInviteeDetailsResponse(
    Guid Id,
    UserSummaryResponse User,
    IReadOnlyCollection<RequirementCompletionResponse> Requirements
);