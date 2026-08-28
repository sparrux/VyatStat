using System.Diagnostics.CodeAnalysis;
using Hub.Domain.Common;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.ValueObjects;

public enum TextFormat
{
    PlainText = 1,
    Html = 2
}

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public sealed class RichText : ValueObject
{
    RichText() { }
    
    public RichText(string text, TextFormat format)
    {
        Text = text;
        Format = format;
    }
    
    public string Text { get; private set; }
    public TextFormat Format { get; private set; }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Text;
        yield return Format;
    }
}