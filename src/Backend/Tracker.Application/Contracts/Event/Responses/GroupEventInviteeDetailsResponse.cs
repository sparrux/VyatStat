using Tracker.Application.Contracts.User.Responses;
using Tracker.Domain.GroupEvents.Events;

namespace Tracker.Application.Contracts.Event.Responses;

public sealed record GroupEventInviteeDetailsResponse(
    Guid Id,
    UserSummaryResponse User,
    IReadOnlyCollection<InviteeRequirementCompletionResponse> Requirements,
    GroupEventRsvpStatus RsvpStatus,
    GroupEventAdmissionStatus AdmissionStatus
);