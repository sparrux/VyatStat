using FluentValidation;
using Hub.Application.Features.Common.Validators;

namespace Hub.Application.Features.Events.Commands.Create;

sealed class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.Request.Title).NotEmpty();
        
        RuleFor(x => x.Request.Dates)
            .SetValidator(new DatesRangeModelValidator());
    }
}