using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Groups.Training;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class TrainingRating : Auditable
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    TrainingRating() { }

    TrainingRating(User user, int rating)
    {
        User = user;
        Rating = rating;
    }
    
    public int Rating { get; private set; }
    
    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public Guid ModuleId { get; private set; }
    public TrainingModule Module { get; private set; }

    internal static Result<TrainingRating> Create(User user, int rating)
    {
        if (rating is < 0 or > 5)
            return Result.Error("Training Rating must be between 0 and 5");

        return new TrainingRating(user, rating);
    }
}