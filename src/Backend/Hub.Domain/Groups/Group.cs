using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Groups.Members;
using Hub.Domain.Groups.Training;

// ReSharper disable CollectionNeverUpdated.Local

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Groups;

public sealed class Group : AggregateRoot
{
    readonly List<GroupRole> _roles = [];
    readonly List<GroupMember> _members = [];
    readonly List<GroupEvent> _groupEvents = [];
    readonly List<TrainingModule> _modules = [];

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    Group() { }

    Group(string name)
    {
        Name = name;
    }
    
    public string Name { get; private set; }

    public IReadOnlyCollection<GroupRole> Roles => _roles;
    public IReadOnlyCollection<GroupMember> Members => _members;
    public IReadOnlyCollection<GroupEvent> GroupEvents => _groupEvents;
    public IReadOnlyCollection<TrainingModule> Modules => _modules;

    public static Result<Group> Create(string name, User creator)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Group name cannot be null or whitespace"));

        var group = new Group(name);
        
        var member = group.AddMember(creator);
        if (!member.IsSuccess) return member.Map();

        var role = group.AddRole(GroupRole.Admin, isSealed: true);
        if (!role.IsSuccess) return role.Map();

        var roleAdded = group.AddMemberRole(member.Value, role.Value);
        if (!roleAdded.IsSuccess) return roleAdded.Map();
        
        return Result.Success(group);
    }

    public Result UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Group Name cannot be null or whitespace"));

        Name = name;
        return Result.Success();
    }

    public Result<GroupRole> AddRole(string name, bool isSealed)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Group Role Name cannot be null or whitespace"));

        if (Roles.Any(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)))
            return Result.Error("Group Role with the same name already exists");

        var role = GroupRole.Create(name, isSealed);
        if (!role.IsSuccess) return role;

        _roles.Add(role.Value);
        return role;
    }

    public Result<TrainingModule> AddTraining(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Group Training Module cannot be null or whitespace"));

        var module = TrainingModule.Create(name, description);
        if (!module.IsSuccess) return module;
        
        _modules.Add(module.Value);
        return module;
    }

    public Result RemoveTraining(TrainingModule module) =>
        _modules.Remove(module) 
            ? Result.Success() 
            : Result.NotFound("Training Module not found");

    public Result<TrainingSkill> AddTrainingSkill(
        TrainingModule module, User user, string name, string? description)
    {
        if (Modules.All(x => x != module))
            return Result.NotFound("Training Module not found");

        return module.AddSkill(user, name, description);
    }

    public Result RemoveTrainingSkill(TrainingModule module, TrainingSkill skill)
    {
        if (Modules.All(x => x != module))
            return Result.NotFound("Training Module not found");
        
        return module.RemoveSkill(skill);
    }
    
    public Result<TrainingRating> AddTrainingRating(
        TrainingModule module, User user, int rating)
    {
        if (Modules.All(x => x != module))
            return Result.NotFound("Training Module not found");

        return module.AddRating(user, rating);
    }

    public Result RemoveTrainingRating(TrainingModule module, TrainingRating rating)
    {
        if (Modules.All(x => x != module))
            return Result.NotFound("Training Module not found");

        return module.RemoveRating(rating);
    }

    public Result<GroupMember> AddMember(User user) =>
        GroupMember.Create(user, this)
            .Map(x =>
            {
                _members.Add(x);
                return x;
            });

    public Result<GroupMemberRole> AddMemberRole(GroupMember member, GroupRole role)
    {
        if (_members.All(x => x != member))
            return Result.NotFound("Group Member not found");
        
        if (Roles.All(x => x != role))
            return Result.NotFound("Group Role not found");
        
        return role.AddMember(member);
    }
    
    public Result RemoveMemberRole(GroupMemberRole memberRole, GroupRole role)
    {
        if (Roles.All(x => x != role))
            return Result.NotFound("Group Role not found");

        return role.RemoveMember(memberRole);
    }

    public Result RemoveMember(GroupMember member) => 
        !_members.Remove(member) 
            ? Result.NotFound("Group member not found") 
            : Result.Success();
}