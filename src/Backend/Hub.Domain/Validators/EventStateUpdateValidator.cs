using FluentValidation;
using Hub.Domain.Events;

namespace Hub.Domain.Validators;

public sealed class EventStateUpdateValidator : AbstractValidator<Event>
{
    public EventStateUpdateValidator(EventState newState)
    {
        RuleFor(x => x)
            .Custom((ev, context) =>
            {
                if (Event.IsDraft(newState) && Event.IsOngoing(ev.State))
                    context.AddFailure("Cannot set event as draft when ongoing");

                if (Event.IsDraft(newState) && Event.IsFinished(ev.State))
                    context.AddFailure("Cannot set event as draft when finished");

                if (Event.IsOngoing(newState) && Event.IsFinished(ev.State))
                    context.AddFailure("Cannot set event as ongoing when finished");
            });
    }
}