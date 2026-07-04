using FluentResults;
using Tracker.Domain.Common;
using Tracker.Domain.GroupEvents.Invitees;

namespace Tracker.Domain.GroupEvents;

public sealed class GroupEventRequirement : Entity
{
    readonly List<GroupEventInviteeRequirementCompletion> _completions = [];
    
    GroupEventRequirement() { }

    public string Title { get; private set; }
    public string? Description { get; private set; }
    public bool IsMandatory { get; private set; }
    public int SortOrder { get; private set; }
    
    public Guid EventId { get; }
    public GroupEvent Event { get; }

    public IReadOnlyCollection<GroupEventInviteeRequirementCompletion> Completions => 
        _completions;
    
    public static Result<GroupEventRequirement> Create(
        string title, string? description, bool isMandatory, int sortOrder)
    {
        if (ValidateTitle(title) is { IsSuccess: false } validation)
            return validation;
        
        return new GroupEventRequirement
        {
            Title = title,
            Description = description,
            IsMandatory = isMandatory,
            SortOrder = sortOrder
        };
    }

    public Result UpdateRequirement(string title, string? description, bool isMandatory)
    {
        if (ValidateTitle(title) is { IsSuccess: false } validation)
            return validation;
        
        Title = title;
        Description = description;
        IsMandatory = isMandatory;
        
        return Result.Ok();
    }
    
    static Result ValidateTitle(string title)
    {
        return Result.FailIf(string.IsNullOrWhiteSpace(title), "Title is required");
    }
}