using FluentResults;
using Tracker.Domain.Common;

namespace Tracker.Domain.GroupEvents.Invitees;

public sealed class GroupEventInviteeRequirementCompletion : Auditable
{
    GroupEventInviteeRequirementCompletion() { }
    
    GroupEventInviteeRequirementCompletion(
        GroupEventInvitee invitee, GroupEventRequirement requirement)
    {
        Invitee = invitee;
        Requirement = requirement;
    }
    
    public Guid InviteeId { get; }
    public GroupEventInvitee Invitee { get; }
    
    public Guid RequirementId { get; }
    public GroupEventRequirement Requirement { get; }
    
    public GroupEventInviteeRequirementStatus CompletionStatus { get; private set; }
    
    public static Result<GroupEventInviteeRequirementCompletion> Create(
        GroupEventInvitee invitee, GroupEventRequirement requirement)
    {
        return Result.Ok(new GroupEventInviteeRequirementCompletion(invitee, requirement));
    }
    
    public Result<GroupEventInviteeRequirementCompletion> UpdateCompletionStatus(
        GroupEventInviteeRequirementStatus status)
    {
        CompletionStatus = status;
        return Result.Ok();
    }
}