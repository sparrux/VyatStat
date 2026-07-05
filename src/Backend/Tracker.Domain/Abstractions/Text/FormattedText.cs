using System.Diagnostics.CodeAnalysis;
using FluentResults;
using Tracker.Domain.Common;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Tracker.Domain.Abstractions.Text;

public abstract class FormattedText : Auditable
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    protected FormattedText() { }
    
    protected FormattedText(string text, TextFormat format)
    {
        Text = text;
        Format = format;
    }
    
    public string Text { get; private set; }
    public TextFormat Format { get; private set; }
    
    public Result Update(string text, TextFormat format)
    {
        if (ValidateText(text) is { IsSuccess: false } validate)
            return validate;
        
        Text = text;
        Format = format;
        
        return Result.Ok();
    }
    
    protected static Result ValidateText(string? text)
    {
        return Result.FailIf(text == null, "Text cannot be null");
    }
}