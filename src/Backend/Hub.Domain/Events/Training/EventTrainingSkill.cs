using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Events.Participants;
using Hub.Domain.Groups.Training;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events.Training;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventTrainingSkill : Auditable
{
    EventTrainingSkill() { }

    public Guid SkillId { get; private set; }
    public TrainingSkill Skill { get; private set; }

    public Guid AssessorId { get; private set; }
    public EventParticipant Assessor { get; private set; }
    
    public Guid EventId { get; private set; }
    public Event Event { get; private set; }

    public static Result<EventTrainingSkill> Create(TrainingSkill skill, EventParticipant assessor)
    {
        return new EventTrainingSkill
        {
            Skill = skill,
            Assessor = assessor
        };
    }
}