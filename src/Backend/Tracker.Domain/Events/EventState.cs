namespace Tracker.Domain.Events;

public enum EventState
{
    Draft = 0,
    
    RegistrationOpen = 1,
    RegistrationClosed = 2,
    
    InProgress = 3,
    
    Completed = 4,
    Cancelled = 5
}