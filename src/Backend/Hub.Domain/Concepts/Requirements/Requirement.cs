using Ardalis.Result;
using Hub.Domain.Common;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Concepts.Requirements;

public abstract class Requirement : Auditable
{
    protected Requirement() { }

    protected Requirement(string title, string? description, bool isMandatory, RequirementVerificationMode verificationMode)
    {
        Title = title;
        Description = description;
        IsMandatory = isMandatory;
        VerificationMode = verificationMode;
    }
    
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public bool IsMandatory { get; private set; }
    public RequirementVerificationMode VerificationMode { get; private set; }
    
    internal Result UpdateRequirement(
        string title, string? description, bool isMandatory, RequirementVerificationMode verificationMode)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Invalid(new ValidationError("Title cannot be null or whitespace"));
        
        Title = title;
        Description = description;
        IsMandatory = isMandatory;
        VerificationMode = verificationMode;
        
        return Result.Success();
    }
}