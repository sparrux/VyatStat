using System.Diagnostics.CodeAnalysis;
using FluentResults;
using Tracker.Domain.Common;
using Tracker.Domain.Events;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Tracker.Domain.Groups;

public sealed class Group : Auditable
{
    readonly List<GroupEvent> _groupEvents = [];
    readonly List<GroupMember> _members = [];

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    Group() { }

    Group(string name)
    {
        Name = name;
    }
    
    public string Name { get; private set; }

    public IReadOnlyCollection<GroupMember> Members => _members;
    public IReadOnlyCollection<GroupEvent> GroupEvents => _groupEvents;

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
    
    public Result<GroupEvent> CreateEvent(string title, DateTimeOffset start, DateTimeOffset end)
    {
        var @event = Event.CreateDraft(title, start, end);
        
        if (@event.IsFailed)
            return @event.ToResult();

        var groupEvent = GroupEvent.Create(this, @event.Value);
        
        _groupEvents.Add(groupEvent.Value);
        return Result.Ok(groupEvent.Value);
    }
    
    public Result RemoveEvent(GroupEvent groupEvent)
    {
        if (!_groupEvents.Remove(groupEvent))
            return Result.Fail("Group event not found");
        
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