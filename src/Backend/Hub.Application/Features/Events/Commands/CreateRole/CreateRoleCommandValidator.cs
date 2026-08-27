using FluentValidation;

namespace Hub.Application.Features.Events.Commands.CreateRole;

sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
