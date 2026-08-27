using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Events.Goals;
using Hub.Domain.Events.Handlers;
using Hub.Domain.Events.Participants;
using Hub.Domain.Events.Reports;
using Hub.Domain.Events.Requirements;
using Hub.Domain.Events.Training;
using Hub.Domain.Extensions;
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
    readonly List<EventRole> _roles = [];
    readonly List<EventGoal> _goals = [];
    readonly List<EventReport> _reports = [];
    readonly List<GroupEvent> _groupEvents = [];
    readonly List<EventParticipant> _participants = [];
    readonly List<EventRequirement> _requirements = [];
    readonly List<EventTrainingSkill> _skillsAssessments = [];
    readonly List<EventTrainingRating> _rates = [];

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    Event() { }

    Event(string title, DatesRange dates)
    {
        Title = title;
        DatesRange = dates;
        State = EventState.Draft;
    }

    public string Title { get; private set; }
    public RichText? Description { get; private set; }
    public DatesRange DatesRange { get; private set; }
    public EventState State { get; internal set; }
    public EventLocation? Location { get; private set; }
    
    public IReadOnlyCollection<EventRole> Roles => _roles;
    public IReadOnlyCollection<EventGoal> Goals => _goals;
    public IReadOnlyCollection<EventReport> Reports => _reports;
    public IReadOnlyCollection<GroupEvent> GroupEvents => _groupEvents;
    public IReadOnlyCollection<EventParticipant> Participants => _participants;
    public IReadOnlyCollection<EventRequirement> Requirements => _requirements;
    public IReadOnlyCollection<EventTrainingRating> Rates => _rates;
    public IReadOnlyCollection<EventTrainingSkill> SkillsAssessments => _skillsAssessments;

    public static Result<Event> CreateDraft(
        User organizer, string title, DatesRange dates)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Invalid(new ValidationError("Event title cannot be null or whitespace"));

        var evt = new Event(title, dates);

        var role = evt.AddRole(EventRole.Organizer, isSealed: true);
        if (!role.IsSuccess) return role.Map();

        var participant = evt.AddParticipant(organizer, ignoreEventState: true);
        if (!participant.IsSuccess) return participant.Map();

        var participantRole = role.Value.AddParticipant(participant.Value);
        if (!participantRole.IsSuccess) return participantRole.Map();
        
        return Result.Success(evt);
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

    public Result<EventRole> AddRole(string name, bool isSealed)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Event role name cannot be null or whitespace"));
        
        if (Roles.Any(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)))
            return Result.Error("Event Role with the same name already exists");
        
        var role = EventRole.Create(name, isSealed);
        if (!role.IsSuccess) return role;
        
        _roles.Add(role.Value);
        return Result.Success(role.Value);
    }
    
    public Result RemoveRole(EventRole role)
    {
        if (role.IsSealed)
            return Result.Error("Event Role cannot be removed because is sealed");
        
        if (Roles.Count <= 1)
            return Result.Error("Event cannot have less than one role");
        
        return _roles.Remove(role)
            ? Result.Success()
            : Result.Error("Event Role is not found");
    }

    public Result<EventParticipantRole> AddParticipantRole(EventRole role, EventParticipant participant)
    {
        if (!Roles.Contains(role))
            return Result.NotFound("Event Role is not found");
        
        if (!Participants.Contains(participant))
            return Result.NotFound("Event Participant is not found");
        
        if (Participants.AlreadyInRole(role, participant.UserId))
            return Result.Error("Participant already has this role");

        return role.AddParticipant(participant);
    }
    
    public Result RemoveParticipantRole(EventRole role, EventParticipantRole participantRole)
    {
        if (!Roles.Contains(role))
            return Result.NotFound("Event Role is not found");

        return role.RemoveParticipant(participantRole);
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
        
        if (_goals.All(x => x != goal))
            return Result.NotFound("Event goal is not found");

        return goal.UpdateName(name);
    }

    public Result<EventGoalTask> AddGoalTask(EventGoal goal, string taskName)
    {
        if (_goals.All(x => x != goal))
            return Result.NotFound("Event Goal is not found");

        return goal.CreateTask(taskName);
    }
    
    public Result UpdateGoalTaskName(EventGoal goal, EventGoalTask task, string name)
    {
        if (_goals.All(x => x != goal))
            return Result.NotFound("Event Goal is not found");
        
        if (goal.Tasks.All(x => x != task))
            return Result.NotFound("Goal Task is not found");

        return task.UpdateName(name);
    }
    
    public Result RemoveGoalTask(EventGoal goal, EventGoalTask task)
    {
        if (_goals.All(x => x != goal))
            return Result.NotFound("Event Goal is not found");

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
    
    public Result<EventParticipant> AddParticipant(User user, bool ignoreEventState = false)
    {
        if (!IsRegistrationOpen(State) && !ignoreEventState)
            return Result.Error("Event registration must be open");
        
        if (Participants.Any(x => x.UserId == user.Id))
            return Result.Error("Participant already exists");

        var participant = EventParticipant.Create(user);
        if (!participant.IsSuccess) return participant;

        foreach (var requirement in Requirements.Where(r => r.AssignmentPolicy > RequirementAssignmentPolicy.Manual))
        {
            participant.Value.Assign(requirement);
        }

        _participants.Add(participant.Value);
        return participant;
    }
    
    public Result<EventRequirement> AddRequirement(string title, string? description, RequirementAssignmentPolicy assignmentPolicy)
    {
        if (IsFinished(State))
            return Result.Error("Event is finished already");

        var createResult = EventRequirement.Create(title, description, assignmentPolicy);
        if (!createResult.IsSuccess) return createResult;

        var requirement = createResult.Value;

        if (assignmentPolicy is RequirementAssignmentPolicy.AutomaticForAllParticipants)
            foreach (var participant in Participants)
                participant.Assign(requirement);

        _requirements.Add(requirement);
        return Result.Success(requirement);
    }
    
    public Result UpdateRequirement(EventRequirement requirement, string title, string? description)
    {
        if (IsFinished(State))
            return Result.Error("Event is finished already");
        
        if (Requirements.All(x => x != requirement))
            return Result.NotFound("Event requirement is not found");
        
        return requirement.UpdateRequirement(title, description);
    }

    public Result<EventRequirementRoleVerifier> AddRequirementRoleVerifier(EventRequirement requirement, EventRole role, bool isRequired)
    {
        if (IsFinished(State))
            return Result.Error("Event is finished");
        
        return requirement.AddRoleVerifier(role, isRequired);
    }
    
    public Result VerifyRequirementByActor(Guid participantUser, Guid requirement, Guid actor)
    {
        if (!IsOngoing(State))
            return Result.Error("Event must be ongoing");
        
        var handler = new RequirementVerificationHandler(this);
        return handler.SubmitVerification(new VerifyByActor(participantUser, requirement, actor));
    }
    
    public Result VerifyRequirementByAutomatic(Guid participantUser, Guid requirement)
    {
        if (!IsOngoing(State))
            return Result.Error("Event must be ongoing");
        
        var handler = new RequirementVerificationHandler(this);
        return handler.SubmitVerification(new VerifyByAutomatic(participantUser, requirement));
    }

    public Result RemoveRequirement(EventRequirement requirement)
    {
        if (!IsDraft(State))
            return Result.Error("Event is not in draft state");
        
        return _requirements.Remove(requirement)
            ? Result.Success()
            : Result.NotFound("Event requirement is not found");
    }

    public Result<EventReport> AddReport(string title, RichText body, EventParticipant participant)
    {
        var report = EventReport.Create(title, body, participant);
        if (!report.IsSuccess) return report;
        
        _reports.Add(report);
        return report;
    }
    
    public Result RemoveReport(EventReport report) =>
        !_reports.Remove(report)
            ? Result.NotFound("Event Report is not found")
            : Result.Success();
    
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