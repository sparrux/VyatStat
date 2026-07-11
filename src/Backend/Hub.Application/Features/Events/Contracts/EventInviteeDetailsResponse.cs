using Hub.Application.Features.Common.Contracts;
using Hub.Domain.Events.Invitees;

namespace Hub.Application.Features.Events.Contracts;

public sealed record EventInviteeDetailsResponse(
    Guid Id,
    UserSummaryResponse User,
    IReadOnlyCollection<RequirementCompletionResponse> Requirements,
    EventInviteeRsvpStatus RsvpStatus,
    EventAdmissionStatus AdmissionStatus
);