using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Hub.Domain.Common;
using Hub.Domain.Validators;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Groups;

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
        var nameValidation = new GroupNameValidator().Validate(name);
        if (!nameValidation.IsValid)
            return Result.Invalid(nameValidation.AsErrors());
        
        return Result.Success(new Group(name));
    }

    public Result UpdateName(string name)
    {
        var nameValidation = new GroupNameValidator().Validate(name);
        if (!nameValidation.IsValid)
            return Result.Invalid(nameValidation.AsErrors());

        Name = name;
        return Result.Success();
    }
    
    public Result<GroupMember> AddMember(User user) =>
        GroupMember.Create(user, this)
            .Map(x =>
            {
                _members.Add(x);
                return x;
            });

    public Result RemoveMember(GroupMember member) => 
        !_members.Remove(member) 
            ? Result.NotFound("Group member not found") 
            : Result.Success();
}