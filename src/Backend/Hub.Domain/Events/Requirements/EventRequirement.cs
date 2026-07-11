using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Hub.Domain.Concepts.Requirements;
using Hub.Domain.Validators;

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
        ConfirmationMode confirmationMode
    ) : base(title, description, isMandatory, confirmationMode) { }

    public Guid EventId { get; private set; }
    public Event Event { get; private set; }

    public IReadOnlyCollection<EventRequirementCompletion> Completions => _completions;
    
    internal static Result<EventRequirement> Create(
        string title, string? description, bool isMandatory, ConfirmationMode confirmationMode)
    {
        var titleValidation = new RequirementTitleValidator().Validate(title);
        if (!titleValidation.IsValid)
            return Result.Invalid(titleValidation.AsErrors());
        
        return new EventRequirement(title, description, isMandatory, confirmationMode);
    }
}