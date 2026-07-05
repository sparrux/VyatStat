using Tracker.Application.Contracts.Requirements.Responses;
using Tracker.Application.Contracts.Users.Responses;
using Tracker.Domain.Events.Invitees;

namespace Tracker.Application.Contracts.Invitees.Responses;

public sealed record EventInviteeDetailsResponse(
    Guid Id,
    UserSummaryResponse User,
    IReadOnlyCollection<RequirementCompletionResponse> Requirements,
    EventInviteeRsvpStatus RsvpStatus,
    EventAdmissionStatus AdmissionStatus
);