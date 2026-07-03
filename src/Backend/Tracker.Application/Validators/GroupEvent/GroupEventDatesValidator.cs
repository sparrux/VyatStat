using System.Linq.Expressions;
using FluentValidation;

namespace Tracker.Application.Validators.GroupEvent;

sealed class GroupEventDatesValidator<T> : AbstractValidator<T>
{
    public GroupEventDatesValidator(
        Expression<Func<T, DateTimeOffset>> startDateExpression,
        Expression<Func<T, DateTimeOffset>> endDateExpression)
    {
        RuleFor(startDateExpression)
            .LessThan(endDateExpression)
            .WithMessage("Start date must be before end date");
    }
}
