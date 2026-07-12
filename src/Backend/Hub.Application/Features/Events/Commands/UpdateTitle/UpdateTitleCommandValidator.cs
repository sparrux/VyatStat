using FluentValidation;

namespace Hub.Application.Features.Events.Commands.UpdateTitle;

sealed class UpdateTitleCommandValidator : AbstractValidator<UpdateTitleCommand>
{
    public UpdateTitleCommandValidator()
    {
        RuleFor(x => x.Request.NewTitle).NotEmpty();
    }
}