using FluentValidation;
using Hub.Application.Features.Common.Validators;

namespace Hub.Application.Features.Events.Commands.UpdateDates;

sealed class UpdateDatesCommandValidator : AbstractValidator<UpdateDatesCommand>
{
    public UpdateDatesCommandValidator()
    {
        RuleFor(x => x.Request)
            .SetValidator(new DatesRangeModelValidator());
    }
}
