using FluentValidation;

namespace Hub.Application.Features.Events.Commands.UpdateState;

sealed class UpdateStateCommandValidator : AbstractValidator<UpdateStateCommand>
{
    public UpdateStateCommandValidator()
    {
        RuleFor(x => x.NewState).IsInEnum();
    }
}