using FluentValidation;

namespace Hub.Application.Features.Events.Commands.CreateRequirement;

sealed class CreateRequirementCommandValidator : AbstractValidator<CreateRequirementCommand>
{
    public CreateRequirementCommandValidator()
    {
        RuleFor(x => x.Request.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Request.Description)
            .MaximumLength(2000);

        RuleFor(x => x.Request.AssignmentPolicy)
            .IsInEnum();
    }
}
