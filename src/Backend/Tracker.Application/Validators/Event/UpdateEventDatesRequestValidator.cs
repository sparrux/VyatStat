using FluentValidation;
using Tracker.Application.Contracts.Events.Requests;

namespace Tracker.Application.Validators.Event;

sealed class UpdateEventDatesRequestValidator : AbstractValidator<UpdateEventDatesRequest>
{
    public UpdateEventDatesRequestValidator()
    {
        Include(new EventDatesValidator<UpdateEventDatesRequest>(
            x => x.NewStartDate,
            x => x.NewEndDate));
    }
}
