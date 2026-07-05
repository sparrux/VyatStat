using FluentValidation;
using Tracker.Application.Contracts.Events.Requests;

namespace Tracker.Application.Validators.Event;

sealed class EventDescriptionRequestValidator : AbstractValidator<EventDescriptionRequest>
{
    public EventDescriptionRequestValidator()
    {
        RuleFor(x => x.Text)
            .Cascade(CascadeMode.Stop)
            .Must(text => !string.IsNullOrWhiteSpace(text))
            .WithMessage("Text is required");

        RuleFor(x => x.Format)
            .IsInEnum()
            .WithMessage("Invalid text format");
    }
}
