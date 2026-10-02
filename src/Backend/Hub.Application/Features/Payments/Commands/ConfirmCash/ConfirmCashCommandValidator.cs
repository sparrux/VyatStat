using FluentValidation;

namespace Hub.Application.Features.Payments.Commands.ConfirmCash;

sealed class ConfirmCashCommandValidator : AbstractValidator<ConfirmCashCommand>
{
    public ConfirmCashCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty();
    }
}
