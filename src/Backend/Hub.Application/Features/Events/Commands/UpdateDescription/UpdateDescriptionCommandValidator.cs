using FluentValidation;

namespace Hub.Application.Features.Events.Commands.UpdateDescription;

sealed class UpdateDescriptionCommandValidator : AbstractValidator<UpdateDescriptionCommand>
{
    public UpdateDescriptionCommandValidator()
    {
        RuleFor(x => x.Request.NewDescription)
            .Must(x => !string.IsNullOrWhiteSpace(x.Text));
        
        RuleFor(x => x.Request.NewDescription.Format)
            .IsInEnum();
    }
}