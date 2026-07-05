using Tracker.Domain.Events;

namespace Tracker.Application.Contracts.Events.Requests;

public sealed record EventFilterRequest(
    Guid? OrganizerUserId,
    Guid? GroupId,
    Guid? InviteeUserId,
    EventState? State
);