using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Groups.Training;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class TrainingSkill : Auditable
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    TrainingSkill() { }

    TrainingSkill(User user, string name, string? description)
    {
        User = user;
        Name = name;
        Description = description;
    }
    
    public string Name { get; private set; }
    public string? Description { get; private set; }

    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public Guid ModuleId { get; private set; }
    public TrainingModule Module { get; private set; }

    internal static Result<TrainingSkill> Create(User user, string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Training Skill Name is required"));

        return new TrainingSkill(user, name, description);
    }
}