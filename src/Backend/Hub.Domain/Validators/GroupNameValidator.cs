using FluentValidation;

namespace Hub.Domain.Validators;

public sealed class GroupNameValidator : AbstractValidator<string>
{
    public GroupNameValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage("Group name cannot be null or empty");
    }
}