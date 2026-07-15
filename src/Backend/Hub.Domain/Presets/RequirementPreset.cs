using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Concepts.Requirements;

namespace Hub.Domain.Presets;

public sealed class RequirementPreset : Requirement
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    RequirementPreset() { }

    RequirementPreset(
        string title, 
        string? description, 
        bool isMandatory, 
        RequirementVerificationMode verificationMode
    ) : base(title, description, isMandatory, verificationMode) { }
    
    public static Result<RequirementPreset> Create(
        string title, string? description, bool isMandatory, RequirementVerificationMode verificationMode)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Invalid(new ValidationError("Requirement preset title cannot be null or whitespace"));
        
        return new RequirementPreset(title, description, isMandatory, verificationMode);
    }
}