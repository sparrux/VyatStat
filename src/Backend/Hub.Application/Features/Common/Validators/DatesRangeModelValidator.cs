using FluentValidation;
using Hub.Application.Features.Common.Contracts;

namespace Hub.Application.Features.Common.Validators;

sealed class DatesRangeModelValidator : AbstractValidator<DatesRangeModel>
{
    public DatesRangeModelValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate)
            .WithMessage("Start date must be before end date");
    }
}