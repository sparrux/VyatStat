using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Hub.Domain.Concepts.Requirements;
using Hub.Domain.Validators;

namespace Hub.Domain.Presets;

public sealed class RequirementPreset : Requirement
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    RequirementPreset() { }

    RequirementPreset(
        string title, 
        string? description, 
        bool isMandatory, 
        ConfirmationMode confirmationMode
    ) : base(title, description, isMandatory, confirmationMode) { }
    
    public static Result<RequirementPreset> Create(
        string title, string? description, bool isMandatory, ConfirmationMode confirmationMode)
    {
        var titleValidation = new RequirementTitleValidator().Validate(title);
        if (!titleValidation.IsValid)
            return Result.Invalid(titleValidation.AsErrors());
        
        return new RequirementPreset(title, description, isMandatory, confirmationMode);
    }
}