using FluentValidation;

namespace Hub.Application.Features.Payments.Commands.CancelCash;

sealed class CancelCashCommandValidator : AbstractValidator<CancelCashCommand>
{
    public CancelCashCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty();
    }
}
