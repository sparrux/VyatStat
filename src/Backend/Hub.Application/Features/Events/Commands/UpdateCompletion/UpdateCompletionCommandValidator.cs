using FluentValidation;

namespace Hub.Application.Features.Events.Commands.UpdateCompletion;

sealed class UpdateCompletionCommandValidator : AbstractValidator<UpdateCompletionCommand>
{
    public UpdateCompletionCommandValidator()
    {
        RuleFor(x => x.NewStatus).IsInEnum();
    }
}