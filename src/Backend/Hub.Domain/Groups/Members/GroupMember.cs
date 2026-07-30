using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Groups.Members;

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
public sealed class GroupMember : Auditable
{
    readonly List<GroupMemberRole> _roles = [];

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    GroupMember() { }
    
    GroupMember(User user, Group group)
    {
        User = user;
        UserId = user.Id;
        
        Group = group;
        GroupId = group.Id;
    }

    public User User { get; private set; }
    public Guid UserId { get; private set; }
    
    public Group Group { get; private set; }
    public Guid GroupId { get; private set; }

    public IReadOnlyCollection<GroupMemberRole> Roles => _roles;

    public static Result<GroupMember> Create(User user, Group group) => 
        Result.Success(new GroupMember(user, group));
}