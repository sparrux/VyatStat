using System.Linq.Expressions;
using FluentValidation;

namespace Tracker.Application.Validators.GroupEvent;

sealed class GroupEventTitleValidator<T> : AbstractValidator<T>
{
    const int TitleMaxLength = 200;

    public GroupEventTitleValidator(Expression<Func<T, string>> titleExpression)
    {
        RuleFor(titleExpression)
            .Cascade(CascadeMode.Stop)
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("Event title is required")
            .MaximumLength(TitleMaxLength)
            .WithMessage($"Event title must be at most {TitleMaxLength} characters long");
    }
}
