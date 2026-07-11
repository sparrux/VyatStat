using FluentValidation;

namespace Hub.Domain.Validators;

public sealed class GoalTitleValidator : AbstractValidator<string>
{
    public GoalTitleValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage("Goal title cannot be empty");
    }
}