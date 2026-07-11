using FluentValidation;

namespace Tracker.Domain.Validators;

public sealed class NicknameValidator : AbstractValidator<string>
{
    public NicknameValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage("Nickname cannot be null or empty");
    }
}