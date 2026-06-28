using FluentResults;
using Tracker.Domain.Common;
using Tracker.Domain.GroupEvents.Events;

namespace Tracker.Domain.Groups;

public sealed class Group : Auditable
{
    readonly List<GroupEvent> _events = [];
    readonly List<GroupMember> _members = [];

    Group() { }

    Group(string name)
    {
        Name = name;
    }
    
    public string Name { get; private set; }

    public IReadOnlyCollection<GroupEvent> Events => _events;
    public IReadOnlyCollection<GroupMember> Members => _members;

    public static Result<Group> Create(string name)
    {
        if (ValidateName(name) is { IsSuccess: false } validation)
            return validation;

        return Result.Ok(new Group(name));
    }

    public Result UpdateName(string name)
    {
        if (ValidateName(name) is { IsSuccess: false } validation)
            return validation;

        Name = name;
        
        return Result.Ok();
    }
    
    public Result<GroupMember> AddMember(User user)
    {
        var member = GroupMember.Create(user, this);
        
        if (member.IsFailed)
            return member;
        
        _members.Add(member.Value);
        return Result.Ok(member.Value);
    }
    
    public Result RemoveMember(GroupMember member)
    {
        if (!_members.Remove(member))
            return Result.Fail("Member not found");
        
        return Result.Ok();
    }

    static Result ValidateName(string? name)
    {
        return Result.FailIf(string.IsNullOrWhiteSpace(name), "Group name is required");
    }
}