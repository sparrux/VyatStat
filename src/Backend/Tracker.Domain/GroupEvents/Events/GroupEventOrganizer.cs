using FluentResults;
using Tracker.Domain.Common;

namespace Tracker.Domain.GroupEvents.Events;

public sealed class GroupEventOrganizer : Entity
{
    public GroupEventOrganizer() { }
    
    GroupEventOrganizer(User user)
    {
        User = user;
    }

    public User User { get; }

    public Guid EventId { get; }
    public GroupEvent Event { get; }

    public static Result<GroupEventOrganizer> Create(User user)
    {
        if (ValidateUser(user) is { IsSuccess: false } validation)
            return validation;

        return Result.Ok(new GroupEventOrganizer(user));
    }

    static Result ValidateUser(User? user)
    {
        return Result.FailIf(user is null, "User is required");
    }
}