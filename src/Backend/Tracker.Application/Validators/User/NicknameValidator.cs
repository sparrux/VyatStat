using System.Linq.Expressions;
using FluentValidation;

namespace Tracker.Application.Validators.User;

sealed class NicknameValidator<T> : AbstractValidator<T>
{
    const int NicknameMinLength = 2;
    const int NicknameMaxLength = 100;

    public NicknameValidator(Expression<Func<T, string>> nicknameExpression)
    {
        RuleFor(nicknameExpression)
            .Cascade(CascadeMode.Stop)
            .Must(nickname => !string.IsNullOrWhiteSpace(nickname))
            .WithMessage("Invalid nickname")
            .Length(NicknameMinLength, NicknameMaxLength)
            .WithMessage($"Nickname must be between {NicknameMinLength} and {NicknameMaxLength} characters long");
    }
}
