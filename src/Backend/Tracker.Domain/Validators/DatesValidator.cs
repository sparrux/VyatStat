using System.Linq.Expressions;
using FluentValidation;

namespace Tracker.Domain.Validators;

public sealed class DatesValidator<T> : AbstractValidator<T>
{
    public DatesValidator(
        Expression<Func<T, DateTimeOffset>> startDateExpression,
        Expression<Func<T, DateTimeOffset>> endDateExpression)
    {
        RuleFor(startDateExpression)
            .LessThan(endDateExpression)
            .WithMessage("Start date must be before end date");
    }
}