using FluentValidation;
using Hub.Domain.Events;

namespace Hub.Domain.Validators;

public sealed class EventIsDraftValidator : AbstractValidator<Event>
{
    public EventIsDraftValidator()
    {
        RuleFor(x => x.State)
            .Must(Event.IsDraft)
            .WithMessage("Event is not in draft state");
    }
}