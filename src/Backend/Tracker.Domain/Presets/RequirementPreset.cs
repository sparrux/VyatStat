using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using FluentResults;
using Tracker.Domain.Abstractions;
using Tracker.Domain.Abstractions.Requirements;

namespace Tracker.Domain.Presets;

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
        if (ValidateTitle(title) is { IsSuccess: false } validation)
            return validation;
        
        return new RequirementPreset(title, description, isMandatory, confirmationMode);
    }
}