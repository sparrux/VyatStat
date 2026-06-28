using FluentResults;
using Tracker.Domain.Common;

namespace Tracker.Domain.Groups;

public sealed class GroupMember : Auditable
{
    public GroupMember() { }
    
    GroupMember(User user, Group group)
    {
        User = user;
        UserId = user.Id;
        
        Group = group;
        GroupId = group.Id;
    }

    public User User { get; }
    public Guid UserId { get; }
    
    public Group Group { get; }
    public Guid GroupId { get; }

    public static Result<GroupMember> Create(User user, Group group)
    {
        if (ValidateUser(user).Bind(() => ValidateGroup(group)) is { IsSuccess: false } validation)
            return validation;

        return Result.Ok(new GroupMember(user, group));
    }
    
    static Result ValidateUser(User? user)
    {
        return Result.FailIf(user is null, "User is required");
    }

    static Result ValidateGroup(Group? group)
    {
        return Result.FailIf(group is null, "Group is required");
    }
}