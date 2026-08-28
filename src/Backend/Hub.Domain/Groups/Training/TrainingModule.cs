using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Groups.Training;

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class TrainingModule : Entity
{
    readonly List<TrainingSkill> _skills = [];
    readonly List<TrainingRating> _ratings = [];

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    TrainingModule() { }

    TrainingModule(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; private set; }
    public string? Description { get; private set; }

    public Guid GroupId { get; private set; }
    public Group Group { get; private set; }

    public IReadOnlyCollection<TrainingSkill> Skills => _skills;
    public IReadOnlyCollection<TrainingRating> Ratings => _ratings;
    
    internal static Result<TrainingModule> Create(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Error("Training Module Name cannot be null or whitespace");

        return new TrainingModule(name, description);
    }

    internal Result<TrainingRating> AddRating(User user, int rating)
    {
        var ratingResult = TrainingRating.Create(user, rating);
        if (!ratingResult.IsSuccess) return ratingResult;

        _ratings.Add(ratingResult.Value);
        return ratingResult;
    }
    
    internal Result RemoveRating(TrainingRating rating) =>
        !_ratings.Remove(rating) 
            ? Result.NotFound("Rating not found") 
            : Result.Success();

    internal Result<TrainingSkill> AddSkill(User user, string name, string? description) => 
        TrainingSkill.Create(user, name, description);

    internal Result RemoveSkill(TrainingSkill skill) =>
        !_skills.Remove(skill) 
            ? Result.NotFound("Skill not found") 
            : Result.Success();
}