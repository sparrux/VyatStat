using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Events.Participants;
using Hub.Domain.ValueObjects;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events.Reports;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventReport : Auditable
{
    EventReport() { }
    
    public string Title { get; private set; }
    public RichText Body { get; private set; }

    public Guid AuthorId { get; private set; }
    public EventParticipant Author { get; private set; }

    public Guid EventId { get; private set; }
    public Event Event { get; private set; }

    internal static Result<EventReport> Create(string title, RichText body, EventParticipant author)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Invalid(new ValidationError("Event Report Name cannot be null or whitespace"));

        return new EventReport
        {
            Title = title,
            Body = body,
            Author = author
        };
    }
}