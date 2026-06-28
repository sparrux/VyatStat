namespace Tracker.Domain.GroupEvents.Events;

public enum GroupEventState
{
    Draft = 0,
    
    RegistrationOpen = 1,
    RegistrationClosed = 2,
    
    InProgress = 3,
    
    Completed = 4,
    Cancelled = 5
}