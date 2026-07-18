using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Concepts.Requirements;
using Hub.Domain.Events.Goals;
using Hub.Domain.Events.Handlers;
using Hub.Domain.Events.Invitees;
using Hub.Domain.Events.Requirements;
using Hub.Domain.Groups;
using Hub.Domain.ValueObjects;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events;

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
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
    public EventState State { get; internal set; }
    public EventLocation? Location { get; private set; }
    
    public IReadOnlyCollection<EventGoal> Goals => _goals;
    public IReadOnlyCollection<GroupEvent> GroupEvents => _groupEvents;
    public IReadOnlyCollection<EventInvitee> Invitees => _invitees;
    public IReadOnlyCollection<EventOrganizer> Organizers => _organizers;
    public IReadOnlyCollection<EventRequirement> Requirements => _requirements;

    public static Result<Event> CreateDraft(User organizer, string title, DatesRange dates)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Invalid(new ValidationError("Event title cannot be null or whitespace"));

        var orgResult = EventOrganizer.Create(organizer);
        if (!orgResult.IsSuccess) return orgResult.Map();
        
        return Result.Success(new Event(orgResult.Value, title, dates));
    }

    public Result UpdateTitle(string title)
    {
        if (IsFinished(State))
            return Result.Error("Event is finished already");
        
        if (string.IsNullOrWhiteSpace(title))
            return Result.Invalid(new ValidationError("Event title cannot be null or whitespace"));

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

        var switchHandler = new EventStateSwitch(this);
        return switchHandler.SwitchState(state);
    }

    public Result UpdateLocation(string? name, Coordinates coordinates)
    {
        if (IsFinished(State))
            return Result.Error("Event is finished already");

        var location = EventLocation.Create(name, coordinates);
        if (!location.IsSuccess) return location.Map();
        
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

    public Result<EventGoal> AddGoal(string name)
    {
        if (IsFinished(State))
            return Result.Error("Event is finished already");
        
        var goal = EventGoal.Create(name);
        if (!goal.IsSuccess) return goal;
        
        _goals.Add(goal.Value);
        return Result.Success(goal.Value);
    }
    
    public Result UpdateGoalName(EventGoal goal, string name)
    {
        if (IsFinished(State))
            return Result.Error("Event is finished already");
        
        if (_goals.All(x => x.Id != goal.Id))
            return Result.Error("Event goal is not found");

        return goal.UpdateName(name);
    }

    public Result<EventGoalTask> CreateGoalTask(EventGoal goal, string taskName)
    {
        if (_goals.All(x => x.Id != goal.Id))
            return Result.Error("Event Goal is not found");

        return goal.CreateTask(taskName);
    }
    
    public Result UpdateGoalTaskName(EventGoal goal, EventGoalTask task, string name)
    {
        if (_goals.All(x => x.Id != goal.Id))
            return Result.Error("Event Goal is not found");
        
        if (goal.Tasks.All(x => x.Id != task.Id))
            return Result.Error("Goal Task is not found");

        return task.UpdateName(name);
    }
    
    public Result RemoveGoalTask(EventGoal goal, EventGoalTask task)
    {
        if (_goals.All(x => x.Id != goal.Id))
            return Result.Error("Event Goal is not found");

        return goal.RemoveTask(task);
    }

    public Result RemoveGoal(EventGoal goal)
    {
        if (IsFinished(State))
            return Result.Error("Event is finished already");
        
        return _goals.Remove(goal)
            ? Result.Success()
            : Result.NotFound("Event Goal is not found");
    }
    
    public Result<EventInvitee> AddInvitee(User user)
    {
        if (!IsRegistrationOpen(State))
            return Result.Error("Event registration must be open");
        
        if (Invitees.Any(x => x.UserId == user.Id))
            return Result.Error("Invitee already exists");

        var inviteeResult = EventInvitee.Create(user);
        if (!inviteeResult.IsSuccess) return inviteeResult;

        var invitee = inviteeResult.Value;

        foreach (var requirement in Requirements)
        {
            invitee.AddCompletion(requirement);
        }

        _invitees.Add(invitee);
        return inviteeResult;
    }
    
    public Result<EventOrganizer> AddOrganizer(User user)
    {
        if (IsFinished(State))
            return Result.Error("Event is finished already");
        
        if (Organizers.Any(x => x.UserId == user.Id))
            return Result.Error("Organizer already exists");

        var organizer = EventOrganizer.Create(user);
        if (!organizer.IsSuccess) return organizer;
        
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
    
    public Result<EventRequirement> AddRequirement(
        string title, string? description, bool isMandatory, RequirementVerificationMode verificationMode)
    {
        if (!IsDraft(State))
            return Result.Error("Event is not in draft state");

        var createResult = EventRequirement.Create(title, description, isMandatory, verificationMode);
        if (!createResult.IsSuccess) return createResult;

        var requirement = createResult.Value;

        _requirements.Add(requirement);
        return Result.Success(requirement);
    }
    
    public Result UpdateRequirement(
        Guid requirementId,
        string title,
        string? description,
        bool isMandatory,
        RequirementVerificationMode verificationMode)
    {
        if (!IsDraft(State))
            return Result.Error("Event is not in draft state");
        
        var requirement = Requirements.FirstOrDefault(r => r.Id == requirementId);
        return requirement is null 
            ? Result.NotFound("Event requirement is not found") 
            : requirement.UpdateRequirement(title, description, isMandatory, verificationMode);
    }
    
    public Result VerifyCompletionByActor(Guid inviteeUser, Guid requirement, Guid actor)
    {
        if (!IsOngoing(State))
            return Result.Error("Event must be ongoing");
        
        var handler = new RequirementVerificationHandler(this);
        return handler.SubmitVerification(new VerifyByActor(inviteeUser, requirement, actor));
    }
    
    public Result VerifyCompletionByAutomatic(Guid inviteeUser, Guid requirement)
    {
        if (!IsOngoing(State))
            return Result.Error("Event must be ongoing");
        
        var handler = new RequirementVerificationHandler(this);
        return handler.SubmitVerification(new VerifyByAutomatic(inviteeUser, requirement));
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
    
    public static bool IsRegistrationOpen(EventState state) =>
        state
            is EventState.RegistrationOpen;

    public static bool IsOngoing(EventState state) =>
        state
            is EventState.RegistrationOpen
            or EventState.RegistrationClosed
            or EventState.InProgress;
}