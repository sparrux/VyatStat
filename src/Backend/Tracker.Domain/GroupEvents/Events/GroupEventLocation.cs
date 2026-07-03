using FluentResults;
using Tracker.Domain.Common;

namespace Tracker.Domain.GroupEvents.Events;

public sealed class GroupEventLocation : Entity
{
    GroupEventLocation() { }
    
    GroupEventLocation(Location location)
    {
        Location = location;
    }
    
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; }
    
    public Guid EventId { get; }
    public GroupEvent Event { get; }

    public static Result<GroupEventLocation> Create(Location location)
    {
        if (ValidateLocation(location) is { IsSuccess: false } validation)
            return validation;

        return Result.Ok(new GroupEventLocation(location));
    }
    
    static Result ValidateLocation(Location? location)
    {
        return Result.FailIf(location is null, "Location is required");
    }
}