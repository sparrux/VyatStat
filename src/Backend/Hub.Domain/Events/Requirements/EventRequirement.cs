using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Concepts.Requirements;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events.Requirements;

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventRequirement : Requirement
{
    readonly List<EventRequirementCompletion> _completions = [];
    
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    EventRequirement() { }

    EventRequirement(
        string title, 
        string? description, 
        bool isMandatory, 
        RequirementVerificationMode verificationMode
    ) : base(title, description, isMandatory, verificationMode) { }

    public Guid EventId { get; private set; }
    public Event Event { get; private set; }

    public IReadOnlyCollection<EventRequirementCompletion> Completions => _completions;
    
    internal static Result<EventRequirement> Create(
        string title, string? description, bool isMandatory, RequirementVerificationMode verificationMode)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Invalid(new ValidationError("Title cannot be null or whitespace"));
        
        return new EventRequirement(title, description, isMandatory, verificationMode);
    }

    public bool IsManualByUserMode() =>
        VerificationMode is RequirementVerificationMode.ManualByUser;
    
    public bool IsManualByOrganizerMode() =>
        VerificationMode is RequirementVerificationMode.ManualByOrganizer;
    
    public bool IsAutomaticMode() =>
        VerificationMode is RequirementVerificationMode.Automatic;
}