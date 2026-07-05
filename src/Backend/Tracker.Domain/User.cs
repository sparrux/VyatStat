using System.Diagnostics.CodeAnalysis;
using FluentResults;
using Tracker.Domain.Common;
using Tracker.Domain.Events;
using Tracker.Domain.Events.Invitees;
using Tracker.Domain.Groups;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Tracker.Domain;

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
public sealed class User : Auditable
{
    readonly List<EventInvitee> _invitees = [];
    readonly List<GroupMember> _memberships = [];
    readonly List<EventOrganizer> _organizers = [];

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    User() { }
    
    User(Guid id, string nickname)
    {
        Id = id;
        Nickname = nickname;
    }

    public string Nickname { get; private set; }

    public IReadOnlyCollection<GroupMember> Memberships => _memberships;
    public IReadOnlyCollection<EventInvitee> Invitees => _invitees;
    public IReadOnlyCollection<EventOrganizer> Organizers => _organizers;

    public static Result<User> Create(Guid id, string nickname)
    {
        if (id == Guid.Empty)
            return Result.Fail("Invalid user id");

        if (ValidateNickname(nickname) is { IsSuccess: false } validation)
            return validation;

        return Result.Ok(new User(id, nickname));
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
        return Result.FailIf(string.IsNullOrWhiteSpace(nickname), "Invalid nickname");
    }
}
