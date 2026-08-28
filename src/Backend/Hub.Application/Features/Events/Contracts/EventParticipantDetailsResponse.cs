using Hub.Application.Features.Users.Contracts;

namespace Hub.Application.Features.Events.Contracts;

public sealed record EventParticipantDetailsResponse(
    UserSummaryResponse User,
    IReadOnlyCollection<RequirementCompletionResponse> Requirements,
    IReadOnlyCollection<EventParticipantRoleResponse> Roles
);
