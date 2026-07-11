using FluentValidation;
using Hub.Application.Features.Common.Validators;
using Hub.Domain.Validators;

namespace Hub.Application.Features.Events.Commands.Create;

sealed class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.Title)
            .SetValidator(new EventTitleValidator());
        
        RuleFor(x => x.Dates)
            .SetValidator(new DatesRangeModelValidator());
    }
}