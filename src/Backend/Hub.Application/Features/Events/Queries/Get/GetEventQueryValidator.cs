using FluentValidation;
using Hub.Application.Features.Common.Validators;

namespace Hub.Application.Features.Events.Queries.Get;

sealed class GetEventQueryValidator : AbstractValidator<GetEventQuery>
{
    public GetEventQueryValidator()
    {
        Include(new GetListQueryValidator());
    }
}