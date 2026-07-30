using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Groups.Members;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class GroupMemberRole : Entity
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    GroupMemberRole() { }

    GroupMemberRole(GroupRole role, GroupMember member)
    {
        Role = role;
        Member = member;
    }

    public Guid RoleId { get; private set; }
    public GroupRole Role { get; private set; }

    public Guid MemberId { get; private set; }
    public GroupMember Member { get; private set; }
    
    internal static Result<GroupMemberRole> Create(
        GroupRole role,
        GroupMember member
    ) => new GroupMemberRole(role, member);
}