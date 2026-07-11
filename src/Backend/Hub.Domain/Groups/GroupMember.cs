using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Groups;

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

    public static Result<GroupMember> Create(User user, Group group) => 
        Result.Success(new GroupMember(user, group));
}