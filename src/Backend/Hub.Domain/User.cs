using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Events;
using Hub.Domain.Events.Invitees;
using Hub.Domain.Groups;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain;

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
public sealed class User : Auditable
{
    readonly List<EventInvitee> _invitees = [];
    readonly List<GroupMember> _memberships = [];
    readonly List<EventOrganizer> _organizers = [];

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    User() { }
    
    User(Guid id, string nickname)
    {
        Id = id;
        Nickname = nickname;
    }

    public string Nickname { get; private set; }

    public IReadOnlyCollection<GroupMember> Memberships => _memberships;
    public IReadOnlyCollection<EventInvitee> Invitees => _invitees;
    public IReadOnlyCollection<EventOrganizer> Organizers => _organizers;

    public static Result<User> Create(Guid id, string nickname)
    {
        if (string.IsNullOrWhiteSpace(nickname))
            return Result.Invalid(new ValidationError("User nickname cannot be null or whitespace"));
        
        return Result.Success(new User(id, nickname));
    }
}
