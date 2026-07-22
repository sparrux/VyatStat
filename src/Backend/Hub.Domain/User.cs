using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Events.Participants;
using Hub.Domain.Groups;
using Hub.Domain.Groups.Training;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain;

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
public sealed class User : Auditable
{
    readonly List<TrainingSkill> _skills = [];
    readonly List<TrainingRating> _ratings = [];
    readonly List<GroupMember> _memberships = [];
    readonly List<EventParticipant> _participants = [];

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    User() { }
    
    User(Guid id, string nickname)
    {
        Id = id;
        Nickname = nickname;
    }

    public string Nickname { get; private set; }

    public IReadOnlyCollection<TrainingSkill> Skills => _skills;
    public IReadOnlyCollection<TrainingRating> Ratings => _ratings;
    
    public IReadOnlyCollection<GroupMember> Memberships => _memberships;
    public IReadOnlyCollection<EventParticipant> Participants => _participants;

    public static Result<User> Create(Guid id, string nickname)
    {
        if (string.IsNullOrWhiteSpace(nickname))
            return Result.Invalid(new ValidationError("User nickname cannot be null or whitespace"));
        
        return Result.Success(new User(id, nickname));
    }
}
