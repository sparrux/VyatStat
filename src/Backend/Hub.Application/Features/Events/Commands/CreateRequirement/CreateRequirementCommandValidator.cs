using FluentValidation;

namespace Hub.Application.Features.Events.Commands.CreateRequirement;

sealed class CreateRequirementCommandValidator : AbstractValidator<CreateRequirementCommand>
{
    public CreateRequirementCommandValidator()
    {
        RuleFor(x => x.Request.Title).NotEmpty();
        RuleFor(x => x.Request.VerificationMode).IsInEnum();
    }
}