using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Hub.Domain.Common;
using Hub.Domain.Concepts.Requirements;
using Hub.Domain.Events.Invitees;
using Hub.Domain.Events.Requirements;
using Hub.Domain.Groups;
using Hub.Domain.Validators;
using Hub.Domain.ValueObjects;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events;

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class Event : AggregateRoot
{
    readonly List<EventGoal> _goals = [];
    readonly List<GroupEvent> _groupEvents = [];
    readonly List<EventInvitee> _invitees = [];
    readonly List<EventOrganizer> _organizers = [];
    readonly List<EventRequirement> _requirements = [];

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    Event() { }

    Event(EventOrganizer organizer, string title, DatesRange dates)
    {
        Title = title;
        DatesRange = dates;
        State = EventState.Draft;
        
        _organizers.Add(organizer);
    }

    public string Title { get; private set; }
    public RichText? Description { get; private set; }
    public DatesRange DatesRange { get; private set; }
    public EventState State { get; private set; }
    public EventLocation? Location { get; private set; }
    
    public IReadOnlyCollection<EventGoal> Goals => _goals;
    public IReadOnlyCollection<GroupEvent> GroupEvents => _groupEvents;
    public IReadOnlyCollection<EventInvitee> Invitees => _invitees;
    public IReadOnlyCollection<EventOrganizer> Organizers => _organizers;
    public IReadOnlyCollection<EventRequirement> Requirements => _requirements;

    public static Result<Event> CreateDraft(User organizer, string title, DatesRange dates)
    {
        var titleValidation = new EventTitleValidator().Validate(title);
        if (!titleValidation.IsValid)
            return Result.Invalid(titleValidation.AsErrors());

        var orgResult = EventOrganizer.Create(organizer);
        if (!orgResult.IsSuccess)
            return orgResult.Map();
        
        return Result.Success(new Event(orgResult.Value, title, dates));
    }

    public Result UpdateTitle(string title)
    {
        var finishedValidation = new EventNotFinishedValidator().Validate(this);
        if (!finishedValidation.IsValid)
            return Result.Invalid(finishedValidation.AsErrors());
        
        var titleValidation = new EventTitleValidator().Validate(title);
        if (!titleValidation.IsValid)
            return Result.Invalid(titleValidation.AsErrors());

        Title = title;
        return Result.Success();
    }
    
    public Result UpdateDescription(RichText? description)
    {
        var finishedValidation = new EventNotFinishedValidator().Validate(this);
        if (!finishedValidation.IsValid)
            return Result.Invalid(finishedValidation.AsErrors());

        Description = description;
        return Result.Success();
    }

    public Result UpdateDates(DatesRange dates)
    {
        var finishedValidation = new EventNotFinishedValidator().Validate(this);
        if (!finishedValidation.IsValid)
            return Result.Invalid(finishedValidation.AsErrors());

        DatesRange = dates;
        return Result.Success();
    }

    public Result UpdateState(EventState state)
    {
        var finishedValidation = new EventNotFinishedValidator().Validate(this);
        if (!finishedValidation.IsValid)
            return Result.Invalid(finishedValidation.AsErrors());
        
        var stateValidation = new EventStateUpdateValidator(state).Validate(this);
        if (!stateValidation.IsValid)
            return Result.Invalid(stateValidation.AsErrors());
        
        State = state;
        return Result.Success();
    }

    public Result UpdateLocation(string? name, Coordinates coordinates)
    {
        var finishedValidation = new EventNotFinishedValidator().Validate(this);
        if (!finishedValidation.IsValid)
            return Result.Invalid(finishedValidation.AsErrors());

        var location = EventLocation.Create(name, coordinates);
        if (location is { IsSuccess: false })
            return Result.Error(new ErrorList(location.Errors));
        
        Location = location.Value;
        return Result.Success();
    }

    public Result RemoveLocation()
    {
        var finishedValidation = new EventNotFinishedValidator().Validate(this);
        if (!finishedValidation.IsValid)
            return Result.Invalid(finishedValidation.AsErrors());
        
        Location = null;
        return Result.Success();
    }

    public Result AddGoal(EventGoal goal)
    {
        var draftValidation = new EventIsDraftValidator().Validate(this);
        if (!draftValidation.IsValid)
            return Result.Invalid(draftValidation.AsErrors());
        
        _goals.Add(goal);
        return Result.Success();
    }

    public Result RemoveGoal(EventGoal goal)
    {
        var draftValidation = new EventIsDraftValidator().Validate(this);
        if (!draftValidation.IsValid)
            return Result.Invalid(draftValidation.AsErrors());
        
        return _goals.Remove(goal)
            ? Result.Success()
            : Result.NotFound("Event goal is not found");
    }
    
    public Result<EventInvitee> AddInvitee(User user)
    {
        var finishedValidation = new EventNotFinishedValidator().Validate(this);
        if (!finishedValidation.IsValid)
            return Result.Invalid(finishedValidation.AsErrors());

        var invitee = EventInvitee.Create(user);
        if (!invitee.IsSuccess)
            return Result.Error(new ErrorList(invitee.Errors));

        _invitees.Add(invitee.Value);
        return invitee;
    }
    
    public Result<EventOrganizer> AddOrganizer(User user)
    {
        var finishedValidation = new EventNotFinishedValidator().Validate(this);
        if (!finishedValidation.IsValid)
            return Result.Invalid(finishedValidation.AsErrors());

        var organizer = EventOrganizer.Create(user);
        if (!organizer.IsSuccess)
            return Result.Error(new ErrorList(organizer.Errors));
        
        _organizers.Add(organizer.Value);
        return organizer;
    }
    
    public Result RemoveOrganizer(EventOrganizer organizer)
    {
        var finishedValidation = new EventNotFinishedValidator().Validate(this);
        if (!finishedValidation.IsValid)
            return Result.Invalid(finishedValidation.AsErrors());

        return _organizers.Remove(organizer)
            ? Result.Success()
            : Result.NotFound("Event organizer is not found");
    }
    
    public Result AddRequirement(EventRequirement requirement)
    {
        var draftValidation = new EventIsDraftValidator().Validate(this);
        if (!draftValidation.IsValid)
            return Result.Invalid(draftValidation.AsErrors());
        
        _requirements.Add(requirement);
        return Result.Success();
    }
    
    public Result UpdateRequirement(
        Guid requirementId,
        string title,
        string? description,
        bool isMandatory,
        ConfirmationMode confirmationMode)
    {
        var draftValidation = new EventIsDraftValidator().Validate(this);
        if (!draftValidation.IsValid)
            return Result.Invalid(draftValidation.AsErrors());
        
        var requirement = Requirements.FirstOrDefault(r => r.Id == requirementId);
        return requirement is null 
            ? Result.NotFound("Event requirement is not found") 
            : requirement.UpdateRequirement(title, description, isMandatory, confirmationMode);
    }
    
    public Result RemoveRequirement(EventRequirement requirement)
    {
        var draftValidation = new EventIsDraftValidator().Validate(this);
        if (!draftValidation.IsValid)
            return Result.Invalid(draftValidation.AsErrors());
        
        return _requirements.Remove(requirement)
            ? Result.Success()
            : Result.NotFound("Event requirement is not found");
    }
    
    public static bool IsDraft(EventState state) =>
        state
            is EventState.Draft;

    public static bool IsFinished(EventState state) =>
        state 
            is EventState.Completed 
            or EventState.Cancelled;

    public static bool IsOngoing(EventState state) =>
        state
            is EventState.RegistrationOpen
            or EventState.RegistrationClosed
            or EventState.InProgress;
}