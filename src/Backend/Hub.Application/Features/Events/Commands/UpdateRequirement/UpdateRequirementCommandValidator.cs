using FluentValidation;

namespace Hub.Application.Features.Events.Commands.UpdateRequirement;

sealed class UpdateRequirementCommandValidator : AbstractValidator<UpdateRequirementCommand>
{
    public UpdateRequirementCommandValidator()
    {
        RuleFor(x => x.Request.Title).NotNull();
        RuleFor(x => x.Request.ConfirmationMode).IsInEnum();
    }
}