namespace Hub.Domain.Events.Requirements;

public enum EventRequirementCompletionStatus
{
    Pending = 0,
    
    Completed = 1,
    Waived = 2,
    
    Rejected = 3
}