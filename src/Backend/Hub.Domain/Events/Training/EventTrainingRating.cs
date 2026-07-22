using System.Diagnostics.CodeAnalysis;
using Hub.Domain.Common;
using Hub.Domain.Events.Participants;
using Hub.Domain.Groups.Training;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events.Training;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventTrainingRating : Auditable
{
    EventTrainingRating() { }

    public Guid RaterId { get; private set; }
    public EventParticipant Rater { get; private set; }

    public Guid RatingId { get; private set; }
    public TrainingRating Rating { get; private set; }

    public Guid EventId { get; private set; }
    public Event Event { get; private set; }

    internal static EventTrainingRating Create(EventParticipant rater, TrainingRating rating)
    {
        return new EventTrainingRating
        {
            Rater = rater,
            Rating = rating
        };
    }
}