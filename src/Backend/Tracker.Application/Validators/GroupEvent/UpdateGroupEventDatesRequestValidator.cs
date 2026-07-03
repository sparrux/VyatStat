using FluentValidation;
using Tracker.Application.Contracts.Event.Requests;

namespace Tracker.Application.Validators.GroupEvent;

sealed class UpdateGroupEventDatesRequestValidator : AbstractValidator<UpdateGroupEventDatesRequest>
{
    public UpdateGroupEventDatesRequestValidator()
    {
        Include(new GroupEventDatesValidator<UpdateGroupEventDatesRequest>(
            x => x.NewStartDate,
            x => x.NewEndDate));
    }
}
