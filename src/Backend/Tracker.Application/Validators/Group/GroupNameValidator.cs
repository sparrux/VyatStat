using System.Linq.Expressions;
using FluentValidation;

namespace Tracker.Application.Validators.Group;

sealed class GroupNameValidator<T> : AbstractValidator<T>
{
    const int NameMinLength = 2;
    const int NameMaxLength = 200;

    public GroupNameValidator(Expression<Func<T, string>> nameExpression)
    {
        RuleFor(nameExpression)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Group name is required")
            .Length(NameMinLength, NameMaxLength)
            .WithMessage($"Group name must be between {NameMinLength} and {NameMaxLength} characters long");
    }
}
