using System.Diagnostics.CodeAnalysis;
using FluentResults;
using Tracker.Domain.Common;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Tracker.Domain.Events;

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventOrganizer : Auditable
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    EventOrganizer() { }
    
    EventOrganizer(User user)
    {
        User = user;
    }

    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public Guid EventId { get; private set; }
    public Event Event { get; private set; }

    public static Result<EventOrganizer> Create(User user)
    {
        if (ValidateUser(user) is { IsSuccess: false } validation)
            return validation;

        return Result.Ok(new EventOrganizer(user));
    }

    static Result ValidateUser(User? user)
    {
        return Result.FailIf(user is null, "User is required");
    }
}