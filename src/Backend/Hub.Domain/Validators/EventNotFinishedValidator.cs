using FluentValidation;
using Hub.Domain.Events;

namespace Hub.Domain.Validators;

public sealed class EventNotFinishedValidator : AbstractValidator<Event>
{
    public EventNotFinishedValidator()
    {
        RuleFor(x => x.State)
            .Must(x => !Event.IsFinished(x))
            .WithMessage("Event is already finished");
    }
}