using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Concepts.Roles;
using Hub.Domain.Groups.Members;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Groups;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class GroupRole : Role
{
    public const string Admin = "Admin";
    
    readonly List<GroupMemberRole> _members = [];
    
    GroupRole() { }

    GroupRole(string name, bool isSealed) : base(name, isSealed) { }

    public Guid GroupId { get; private set; }
    public Group Group { get; private set; }

    public IReadOnlyCollection<GroupMemberRole> Members => _members;
    
    internal static Result<GroupRole> Create(string name, bool isSealed)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Group Role Name is required"));
        
        return new GroupRole(name, isSealed);
    }
    
    internal Result<GroupMemberRole> AddMember(GroupMember member)
    {
        var memberRole = GroupMemberRole.Create(this, member);
        if (!memberRole.IsSuccess) return memberRole;
        
        _members.Add(memberRole.Value);
        return memberRole;
    }
    
    internal Result RemoveMember(GroupMemberRole memberRole)
    {
        if (IsSealed)
            return Result.Error("Member Role cannot be removed because sealed");
        
        return !_members.Remove(memberRole)
            ? Result.NotFound("Member Role not found")
            : Result.Success();
    }
}