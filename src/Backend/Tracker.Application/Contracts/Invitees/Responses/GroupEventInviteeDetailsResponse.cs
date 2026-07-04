using Tracker.Application.Contracts.Requirements.Responses;
using Tracker.Application.Contracts.Users.Responses;
using Tracker.Domain.GroupEvents.Invitees;

namespace Tracker.Application.Contracts.Invitees.Responses;

public sealed record GroupEventInviteeDetailsResponse(
    Guid Id,
    UserSummaryResponse User,
    IReadOnlyCollection<InviteeRequirementCompletionResponse> Requirements,
    GroupEventRsvpStatus RsvpStatus,
    GroupEventAdmissionStatus AdmissionStatus
);