using FluentValidation;

namespace Hub.Domain.Validators;

public sealed class EventTitleValidator : AbstractValidator<string>
{
    public EventTitleValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage("Event title cannot be null or empty");
    }
}