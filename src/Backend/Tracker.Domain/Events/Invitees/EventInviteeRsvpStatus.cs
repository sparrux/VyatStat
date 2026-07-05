namespace Tracker.Domain.Events.Invitees;

/// <summary>
/// Represents the user's intention to participate in a group event.
/// </summary>
public enum EventInviteeRsvpStatus
{
    /// <summary>
    /// The user has not made a decision yet.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// The user plans to attend the event.
    /// </summary>
    Going = 1,

    /// <summary>
    /// The user does not plan to attend the event.
    /// </summary>
    NotGoing = 2
}