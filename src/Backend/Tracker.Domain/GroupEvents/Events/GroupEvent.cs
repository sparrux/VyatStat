using FluentResults;
using Tracker.Domain.Common;
using Tracker.Domain.Groups;
using Tracker.Domain.Text;

namespace Tracker.Domain.GroupEvents.Events;

public sealed class GroupEvent : Auditable
{
    readonly List<GroupEventTarget> _targets = [];
    readonly List<GroupEventInvitee> _invitees = [];
    readonly List<GroupEventOrganizer> _organizers = [];
    readonly List<GroupEventRequirement> _requirements = [];

    GroupEvent() { }

    GroupEvent(string title, DateTimeOffset start, DateTimeOffset end)
    {
        Title = title;
        StartDate = start;
        EndDate = end;
        State = GroupEventState.Draft;
        Description = GroupEventDescription.Default;
    }

    public string Title { get; private set; }
    public GroupEventDescription Description { get; private set; }
    
    public DateTimeOffset EndDate { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    
    public GroupEventState State { get; private set; }
    public GroupEventLocation? Location { get; private set; }
    
    public IReadOnlyCollection<GroupEventTarget> Targets => _targets;
    public IReadOnlyCollection<GroupEventInvitee> Invitees => _invitees;
    public IReadOnlyCollection<GroupEventOrganizer> Organizers => _organizers;
    public IReadOnlyCollection<GroupEventRequirement> Requirements => _requirements;

    public Guid GroupId { get; }
    public Group Group { get; }
    
    bool IsFinished => State is GroupEventState.Completed or GroupEventState.Cancelled;

    public static Result<GroupEvent> CreateDraft(string title, DateTimeOffset start, DateTimeOffset end)
    {
        if (ValidateTitle(title).Bind(() => ValidateDates(start, end)) 
            is { IsSuccess: false } validation)
            return validation;

        return new GroupEvent(title, start, end);
    }

    public Result UpdateTitle(string title)
    {
        if (ValidateFinished().Bind(() => ValidateTitle(title)) is { IsSuccess: false } validation)
            return validation;

        Title = title;
        
        return Result.Ok();
    }
    
    public Result UpdateDescription(string text, TextFormat format)
    {
        if (ValidateFinished() is { IsSuccess: false } validation)
            return validation;

        return Description.Update(text, format);
    }
    
    public Result UpdateDates(DateTimeOffset start, DateTimeOffset end)
    {
        if (ValidateFinished().Bind(() => ValidateDates(start, end)) is { IsSuccess: false } validation)
            return validation;

        StartDate = start;
        EndDate = end;
        
        return Result.Ok();
    }
    
    public Result UpdateState(GroupEventState state)
    {
        if (ValidateFinished().Bind(() => ValidateState(state)) is { IsSuccess: false } validation)
            return validation;
        
        State = state;
        
        return Result.Ok();
    }

    public Result UpdateLocation(GroupEventLocation? location)
    {
        if (ValidateFinished() is { IsSuccess: false } validation)
            return validation;
        
        Location = location;
        
        return Result.Ok();
    }

    public Result AddTarget(GroupEventTarget target)
    {
        if (ValidateFinished() is { IsSuccess: false } validation)
            return validation;
        
        _targets.Add(target);
        return Result.Ok();
    }
    
    public Result RemoveTarget(GroupEventTarget target)
    {
        if (ValidateFinished() is { IsSuccess: false } validation)
            return validation;
        
        return Result.FailIf(!_targets.Remove(target), "Target not found");
    }
    
    public Result AddInvitee(GroupEventInvitee invitee)
    {
        if (ValidateFinished() is { IsSuccess: false } validation)
            return validation;
        
        _invitees.Add(invitee);
        return Result.Ok();
    }
    
    public Result AddOrganizer(GroupEventOrganizer organizer)
    {
        if (ValidateFinished() is { IsSuccess: false } validation)
            return validation;
        
        _organizers.Add(organizer);
        return Result.Ok();
    }
    
    public Result RemoveOrganizer(GroupEventOrganizer organizer)
    {
        if (ValidateFinished() is { IsSuccess: false } validation)
            return validation;
        
        return Result.FailIf(!_organizers.Remove(organizer), "Organizer not found");
    }
    
    public Result AddRequirement(GroupEventRequirement requirement)
    {
        if (ValidateFinished() is { IsSuccess: false } validation)
            return validation;
        
        _requirements.Add(requirement);
        return Result.Ok();
    }
    
    public Result RemoveRequirement(GroupEventRequirement requirement)
    {
        if (ValidateFinished() is { IsSuccess: false } validation)
            return validation;
        
        return Result.FailIf(!_requirements.Remove(requirement), "Requirement not found");
    }
    
    static Result ValidateTitle(string title)
    {
        return Result.FailIf(string.IsNullOrWhiteSpace(title), "Title is required");
    }

    static Result ValidateDates(DateTimeOffset start, DateTimeOffset end)
    {
        return Result.FailIf(start >= end, "Start date must be before end date");
    }
    
    Result ValidateState(GroupEventState state)
    {
        return ValidateFinished();
    }
    
    Result ValidateFinished()
    {
        return Result.FailIf(IsFinished, "Cannot update finished event");
    }
}