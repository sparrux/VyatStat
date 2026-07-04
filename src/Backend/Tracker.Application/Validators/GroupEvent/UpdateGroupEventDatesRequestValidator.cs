using FluentValidation;
using Tracker.Application.Contracts.GroupEvents.Requests;

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
