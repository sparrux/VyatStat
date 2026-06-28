using FluentResults;
using Tracker.Domain.Common;
using Tracker.Domain.Groups;

namespace Tracker.Domain;

public sealed class User : Auditable
{
    readonly List<GroupMember> _memberships = [];

    User() { }
    
    User(string nickname)
    {
        Nickname = nickname;
    }
    
    public string Nickname { get; }

    public IReadOnlyCollection<GroupMember> Memberships => _memberships;

    public static Result<User> Create(string nickname)
    {
        if (ValidateNickname(nickname) is { IsSuccess: false } validation)
            return validation;

        return Result.Ok(new User(nickname));
    }

    public Result<GroupMember> CreateMembership(Group group)
    {
        var member = GroupMember.Create(this, group);
        
        if (member.IsFailed)
            return member;
        
        _memberships.Add(member.Value);
        return Result.Ok(member.Value);
    }
    
    public Result RemoveMembership(GroupMember member)
    {
        if (!_memberships.Remove(member))
            return Result.Fail("Member not found");

        return Result.Ok();
    }
    
    static Result ValidateNickname(string nickname)
    {
        return Result.FailIf(string.IsNullOrEmpty(nickname), "Invalid nickname");
    }
}