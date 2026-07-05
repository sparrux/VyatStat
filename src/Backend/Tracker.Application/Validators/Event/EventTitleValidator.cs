using System.Linq.Expressions;
using FluentValidation;

namespace Tracker.Application.Validators.Event;

sealed class EventTitleValidator<T> : AbstractValidator<T>
{
    const int TitleMaxLength = 200;

    public EventTitleValidator(Expression<Func<T, string>> titleExpression)
    {
        RuleFor(titleExpression)
            .Cascade(CascadeMode.Stop)
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("Event title is required")
            .MaximumLength(TitleMaxLength)
            .WithMessage($"Event title must be at most {TitleMaxLength} characters long");
    }
}
