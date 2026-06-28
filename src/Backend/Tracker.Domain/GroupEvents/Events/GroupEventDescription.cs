using FluentResults;
using Tracker.Domain.Common;
using Tracker.Domain.Text;

namespace Tracker.Domain.GroupEvents.Events;

public sealed class GroupEventDescription : Auditable
{
    public GroupEventDescription() { }
    
    GroupEventDescription(string text, TextFormat format)
    {
        Text = text;
        Format = format;
    }
    
    public string Text { get; private set; }
    public TextFormat Format { get; private set; }

    public Guid EventId { get; }
    public GroupEvent Event { get; }

    public static GroupEventDescription Default => 
        new("Has no description", TextFormat.PlainText);

    public static Result<GroupEventDescription> Create(string text, TextFormat format)
    {
        if (ValidateText(text) is { IsSuccess: false } validate)
            return validate;

        return new GroupEventDescription(text, format);
    }

    internal Result Update(string text, TextFormat format)
    {
        if (ValidateText(text) is { IsSuccess: false } validate)
            return validate;
        
        Text = text;
        Format = format;
        
        return Result.Ok();
    }

    static Result ValidateText(string? text)
    {
        return Result.FailIf(text is null, "Text cannot be null or whitespace");
    }
}