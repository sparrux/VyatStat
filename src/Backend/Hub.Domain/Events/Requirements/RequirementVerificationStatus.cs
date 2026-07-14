namespace Hub.Domain.Events.Requirements;

public enum RequirementVerificationStatus
{
    NotCompleted = 0,
    PendingVerification = 1,
    Verified = 2,
    Rejected = 3
}