using System.Diagnostics.CodeAnalysis;
using FluentResults;
using Tracker.Domain.Common;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Tracker.Domain.Groups;

public sealed class GroupMember : Auditable
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    GroupMember() { }
    
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