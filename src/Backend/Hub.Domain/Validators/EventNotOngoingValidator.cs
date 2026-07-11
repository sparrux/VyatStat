using FluentValidation;
using Hub.Domain.Events;

namespace Hub.Domain.Validators;

public sealed class EventNotOngoingValidator : AbstractValidator<Event>
{
    public EventNotOngoingValidator()
    {
        RuleFor(x => x.State)
            .Must(x => !Event.IsOngoing(x))
            .WithMessage("Event is already ongoing");
    }
}