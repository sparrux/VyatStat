using FluentValidation;
using Tracker.Application.Contracts.Events.Requests;

namespace Tracker.Application.Validators.Event;

sealed class UpdateEventTitleRequestValidator : AbstractValidator<UpdateEventTitleRequest>
{
    public UpdateEventTitleRequestValidator()
    {
        Include(new EventTitleValidator<UpdateEventTitleRequest>(x => x.NewTitle));
    }
}
