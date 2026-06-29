using FluentResults;
using Tracker.Domain.Common;

namespace Tracker.Domain.GroupEvents.Events;

public sealed class GroupEventInvitee : Auditable
{
    readonly List<GroupEventInviteeRequirementCompletion> _requirementCompletions = [];

    GroupEventInvitee() { }
    
    GroupEventInvitee(User user)
    {
        User = user;
    }
    
    public User User { get; private set; }
    public GroupEventRsvpStatus RsvpStatus { get; private set; }
    public GroupEventAdmissionStatus AdmissionStatus { get; private set; }

    public IReadOnlyCollection<GroupEventInviteeRequirementCompletion> RequirementCompletions =>
        _requirementCompletions;

    public Guid EventId { get; }
    public GroupEvent Event { get; }

    public static Result<GroupEventInvitee> Create(User user)
    {
        if (ValidateUser(user) is { IsSuccess: false } validation)
            return validation;

        return Result.Ok(new GroupEventInvitee(user));
    }
    
    public Result UpdateRsvpStatus(GroupEventRsvpStatus status)
    {
        RsvpStatus = status;
        return Result.Ok();
    }
    
    public Result UpdateAdmissionStatus(GroupEventAdmissionStatus status)
    {
        AdmissionStatus = status;
        return Result.Ok();
    }

    static Result ValidateUser(User? user)
    {
        return Result.FailIf(user is null, "User is required");
    }
}