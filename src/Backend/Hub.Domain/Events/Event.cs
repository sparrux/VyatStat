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
        if (IsFinished(State))
            return Result.Error("Event is finished already");
        
        var titleValidation = new EventTitleValidator().Validate(title);
        if (!titleValidation.IsValid)
            return Result.Invalid(titleValidation.AsErrors());

        Title = title;
        return Result.Success();
    }
    
    public Result UpdateDescription(RichText description)
    {
        if (IsFinished(State))
            return Result.Error("Event is finished already");

        Description = description;
        return Result.Success();
    }
    
    public Result RemoveDescription()
    {
        if (IsFinished(State))
            return Result.Error("Event is finished already");
        
        Description = null;
        return Result.Success();
    }

    public Result UpdateDates(DatesRange dates)
    {
        if (!IsDraft(State))
            return Result.Error("Event is not in draft state");

        DatesRange = dates;
        return Result.Success();
    }

    public Result UpdateState(EventState state)
    {
        if (IsFinished(State))
            return Result.Error("Event is finished already");
        
        var stateValidation = new EventStateUpdateValidator(state).Validate(this);
        if (!stateValidation.IsValid)
            return Result.Invalid(stateValidation.AsErrors());
        
        State = state;
        return Result.Success();
    }

    public Result UpdateLocation(string? name, Coordinates coordinates)
    {
        if (IsFinished(State))
            return Result.Error("Event is finished already");

        var location = EventLocation.Create(name, coordinates);
        if (location is { IsSuccess: false })
            return Result.Error(new ErrorList(location.Errors));
        
        Location = location.Value;
        return Result.Success();
    }

    public Result RemoveLocation()
    {
        if (IsFinished(State))
            return Result.Error("Event is finished already");
        
        Location = null;
        return Result.Success();
    }

    public Result AddGoal(EventGoal goal)
    {
        if (!IsDraft(State))
            return Result.Error("Event is not in draft state");
        
        _goals.Add(goal);
        return Result.Success();
    }

    public Result RemoveGoal(EventGoal goal)
    {
        if (!IsDraft(State))
            return Result.Error("Event is not in draft state");
        
        return _goals.Remove(goal)
            ? Result.Success()
            : Result.NotFound("Event goal is not found");
    }
    
    public Result<EventInvitee> AddInvitee(User user)
    {
        if (!IsDraft(State))
            return Result.Error("Event is not in draft state");
        
        if (Invitees.Any(x => x.UserId == user.Id))
            return Result.Error("Invitee already exists");

        var invitee = EventInvitee.Create(user);
        if (!invitee.IsSuccess)
            return Result.Error(new ErrorList(invitee.Errors));

        _invitees.Add(invitee.Value);
        return invitee;
    }
    
    public Result<EventOrganizer> AddOrganizer(User user)
    {
        if (IsFinished(State))
            return Result.Error("Event is finished already");
        
        if (Organizers.Any(x => x.UserId == user.Id))
            return Result.Error("Organizer already exists");

        var organizer = EventOrganizer.Create(user);
        if (!organizer.IsSuccess)
            return Result.Error(new ErrorList(organizer.Errors));
        
        _organizers.Add(organizer.Value);
        return organizer;
    }
    
    public Result RemoveOrganizer(EventOrganizer organizer)
    {
        if (IsFinished(State))
            return Result.Error("Event is finished already");

        return _organizers.Remove(organizer)
            ? Result.Success()
            : Result.NotFound("Event organizer is not found");
    }
    
    public Result AddRequirement(EventRequirement requirement)
    {
        if (!IsDraft(State))
            return Result.Error("Event is not in draft state");
        
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
        if (!IsDraft(State))
            return Result.Error("Event is not in draft state");
        
        var requirement = Requirements.FirstOrDefault(r => r.Id == requirementId);
        return requirement is null 
            ? Result.NotFound("Event requirement is not found") 
            : requirement.UpdateRequirement(title, description, isMandatory, confirmationMode);
    }
    
    public Result RemoveRequirement(EventRequirement requirement)
    {
        if (!IsDraft(State))
            return Result.Error("Event is not in draft state");
        
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