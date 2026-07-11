using FluentValidation;

namespace Hub.Domain.Validators;

public sealed class NicknameValidator : AbstractValidator<string>
{
    public NicknameValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage("Nickname cannot be null or empty");
    }
}