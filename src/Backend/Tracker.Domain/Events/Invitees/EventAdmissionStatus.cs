namespace Tracker.Domain.Events.Invitees;

/// <summary>
/// Represents whether a user is allowed to participate in a group event.
/// </summary>
public enum EventAdmissionStatus
{
    /// <summary>
    /// The admission decision has not been made yet.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// The user has been approved and is allowed to participate in the event.
    /// </summary>
    Approved = 1,

    /// <summary>
    /// The user has been rejected and is not allowed to participate in the event.
    /// </summary>
    Rejected = 2
}