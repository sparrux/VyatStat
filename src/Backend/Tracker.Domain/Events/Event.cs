using System.Diagnostics.CodeAnalysis;
using FluentResults;
using Tracker.Domain.Abstractions.Requirements;
using Tracker.Domain.Abstractions.Text;
using Tracker.Domain.Common;
using Tracker.Domain.Events.Invitees;
using Tracker.Domain.Events.Requirements;
using Tracker.Domain.Groups;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Tracker.Domain.Events;

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class Event : Auditable
{
    readonly List<EventGoal> _goals = [];
    readonly List<GroupEvent> _groupEvents = [];
    readonly List<EventInvitee> _invitees = [];
    readonly List<EventOrganizer> _organizers = [];
    readonly List<EventRequirement> _requirements = [];

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    Event() { }

    Event(string title, DateTimeOffset start, DateTimeOffset end)
    {
        Title = title;
        StartDate = start;
        EndDate = end;
        State = EventState.Draft;
        Description = EventDescription
            .Create("Has no description.", TextFormat.PlainText).Value;
    }

    public string Title { get; private set; }
    public EventDescription Description { get; private set; }
    
    public DateTimeOffset EndDate { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    
    public EventState State { get; private set; }
    
    public EventLocation? Location { get; private set; }
    
    public IReadOnlyCollection<EventGoal> Goals => _goals;
    public IReadOnlyCollection<GroupEvent> GroupEvents => _groupEvents;
    public IReadOnlyCollection<EventInvitee> Invitees => _invitees;
    public IReadOnlyCollection<EventOrganizer> Organizers => _organizers;
    public IReadOnlyCollection<EventRequirement> Requirements => _requirements;

    public static Result<Event> CreateDraft(string title, DateTimeOffset start, DateTimeOffset end)
    {
        if (ValidateTitle(title).Bind(() => ValidateDates(start, end)) 
            is { IsSuccess: false } validation)
            return validation;

        return new Event(title, start, end);
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
    
    public Result UpdateState(EventState state)
    {
        if (ValidateState(state) is { IsSuccess: false } validation)
            return validation;
        
        State = state;
        
        return Result.Ok();
    }

    public Result UpdateLocation(string? name, double lat, double lng)
    {
        if (ValidateFinished() is { IsSuccess: false } validation)
            return validation;

        var locationResult = EventLocation.Create(name, lat, lng);

        if (locationResult.IsFailed)
            return locationResult.ToResult();

        Location = locationResult.Value;
        
        return Result.Ok();
    }

    public void RemoveLocation()
    {
        Location = null;
    }

    public Result AddGoal(EventGoal goal)
    {
        if (ValidateOngoing().Bind(ValidateFinished) is { IsSuccess: false } validation)
            return validation;
        
        _goals.Add(goal);
        return Result.Ok();
    }
    
    public Result RemoveGoal(EventGoal goal)
    {
        if (ValidateOngoing().Bind(ValidateFinished) is { IsSuccess: false } validation)
            return validation;
        
        return Result.FailIf(!_goals.Remove(goal), "Goal not found");
    }
    
    public Result<EventInvitee> AddInvitee(User user)
    {
        if (ValidateFinished() is { IsSuccess: false } validation)
            return validation;

        var invitee = EventInvitee.Create(user);
        
        if (invitee.IsFailed)
            return invitee;

        _invitees.Add(invitee.Value);
        
        return invitee;
    }
    
    public Result<EventOrganizer> AddOrganizer(User user)
    {
        if (ValidateFinished() is { IsSuccess: false } validation)
            return validation;

        var org = EventOrganizer.Create(user);

        if (org.IsFailed)
            return org.ToResult();
        
        _organizers.Add(org.Value);
        return org;
    }
    
    public Result RemoveOrganizer(EventOrganizer organizer)
    {
        if (ValidateFinished() is { IsSuccess: false } validation)
            return validation;
        
        return Result.FailIf(!_organizers.Remove(organizer), "Organizer not found");
    }
    
    public Result AddRequirement(EventRequirement requirement)
    {
        if (ValidateOngoing().Bind(ValidateFinished) is { IsSuccess: false } validation)
            return validation;
        
        _requirements.Add(requirement);
        return Result.Ok();
    }
    
    public Result UpdateRequirement(
        Guid requirementId,
        string title,
        string? description,
        bool isMandatory,
        ConfirmationMode confirmationMode)
    {
        if (ValidateOngoing().Bind(ValidateFinished) is { IsSuccess: false } validation)
            return validation;
        
        var requirement = Requirements.FirstOrDefault(r => r.Id == requirementId);
        
        if (requirement is null)
            return Result.Fail("Requirement not found");

        return requirement.UpdateRequirement(title, description, isMandatory, confirmationMode);
    }
    
    public Result RemoveRequirement(EventRequirement requirement)
    {
        if (ValidateOngoing().Bind(ValidateFinished) is { IsSuccess: false } validation)
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
    
    Result ValidateState(EventState state)
    {
        if (IsDraft(state) && IsOngoing(State))
            return Result.Fail("Cannot set event as draft when ongoing");
        
        if (IsDraft(state) && IsFinished(State))
            return Result.Fail("Cannot set event as draft when finished");
        
        if (IsOngoing(state) && IsFinished(State))
            return Result.Fail("Cannot set event as ongoing when finished");
        
        return Result.Ok();
    }
    
    Result ValidateFinished()
    {
        return Result.FailIf(IsFinished(State), "Cannot update finished event");
    }
    
    Result ValidateOngoing()
    {
        return Result.FailIf(IsOngoing(State), "Cannot update ongoing event");
    }

    static bool IsDraft(EventState state) =>
        state
            is EventState.Draft;

    static bool IsFinished(EventState state) =>
        state 
            is EventState.Completed 
            or EventState.Cancelled;

    static bool IsOngoing(EventState state) =>
        state
            is EventState.RegistrationOpen
            or EventState.RegistrationClosed
            or EventState.InProgress;
}