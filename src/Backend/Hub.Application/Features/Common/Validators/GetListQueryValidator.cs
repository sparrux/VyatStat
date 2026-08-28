using FluentValidation;
using Hub.Application.Features.Common.Contracts;

namespace Hub.Application.Features.Common.Validators;

sealed class GetListQueryValidator : AbstractValidator<GetListQuery>
{
    public GetListQueryValidator()
    {
        RuleFor(x => x.Take)
            .LessThanOrEqualTo(30)
            .GreaterThanOrEqualTo(0);
        
        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0);
    }
}