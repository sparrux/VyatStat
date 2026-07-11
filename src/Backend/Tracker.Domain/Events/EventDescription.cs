using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using FluentResults;
using Tracker.Domain.Abstractions.Text;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Tracker.Domain.Events;

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventDescription : FormattedText
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    EventDescription() { }
    
    EventDescription(string text, TextFormat format) : base(text, format) { }
    
    public Guid EventId { get; private set; }
    public Event Event { get; private set; }

    public static Result<EventDescription> Create(string text, TextFormat format)
    {
        if (ValidateText(text) is { IsSuccess: false } validate)
            return validate;

        return new EventDescription(text, format);
    }
}