using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Groups.Training;

// ReSharper disable CollectionNeverUpdated.Local

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Groups;

public sealed class Group : AggregateRoot
{
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

    public IReadOnlyCollection<GroupMember> Members => _members;
    public IReadOnlyCollection<GroupEvent> GroupEvents => _groupEvents;
    public IReadOnlyCollection<TrainingModule> Modules => _modules;

    public static Result<Group> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Group name cannot be null or whitespace"));
        
        return Result.Success(new Group(name));
    }

    public Result UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Group name cannot be null or whitespace"));

        Name = name;
        return Result.Success();
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

    public Result RemoveMember(GroupMember member) => 
        !_members.Remove(member) 
            ? Result.NotFound("Group member not found") 
            : Result.Success();
}