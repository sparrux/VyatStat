using System.Linq.Expressions;
using FluentValidation;

namespace Tracker.Application.Validators.Event;

sealed class EventDatesValidator<T> : AbstractValidator<T>
{
    public EventDatesValidator(
        Expression<Func<T, DateTimeOffset>> startDateExpression,
        Expression<Func<T, DateTimeOffset>> endDateExpression)
    {
        RuleFor(startDateExpression)
            .LessThan(endDateExpression)
            .WithMessage("Start date must be before end date");
    }
}
