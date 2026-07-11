using FluentValidation;

namespace Hub.Domain.Validators;

public sealed class RequirementTitleValidator : AbstractValidator<string>
{
    public RequirementTitleValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage("Requirement title is required");
    }
}