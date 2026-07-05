using FluentResults;
using Tracker.Domain.Common;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Tracker.Domain.Abstractions.Requirements;

public abstract class Requirement : Auditable
{
    protected Requirement() { }

    protected Requirement(string title, string? description, bool isMandatory, ConfirmationMode confirmationMode)
    {
        Title = title;
        Description = description;
        IsMandatory = isMandatory;
        ConfirmationMode = confirmationMode;
    }
    
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public bool IsMandatory { get; private set; }
    public ConfirmationMode ConfirmationMode { get; private set; }
    
    public Result UpdateRequirement(
        string title, string? description, bool isMandatory, ConfirmationMode confirmationMode)
    {
        if (ValidateTitle(title) is { IsSuccess: false } validation)
            return validation;
        
        Title = title;
        Description = description;
        IsMandatory = isMandatory;
        ConfirmationMode = confirmationMode;
        
        return Result.Ok();
    }
    
    protected static Result ValidateTitle(string title)
    {
        return Result.FailIf(string.IsNullOrWhiteSpace(title), "Title is required");
    }
}