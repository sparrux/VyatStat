using Ardalis.Result;

namespace Hub.Domain.Events.Handlers;

sealed class EventStateSwitch(Event evt)
{
    public Result SwitchState(EventState newState)
    {
        if (Event.IsDraft(newState) && Event.IsOngoing(evt.State))
            return Result.Invalid(new ValidationError("Cannot set event as draft when ongoing"));

        if (Event.IsDraft(newState) && Event.IsFinished(evt.State))
            return Result.Invalid(new ValidationError("Cannot set event as draft when finished"));

        if (Event.IsOngoing(newState) && Event.IsFinished(evt.State))
            return Result.Invalid(new ValidationError("Cannot set event as ongoing when finished"));

        if (Event.IsFinished(newState) && Event.IsFinished(evt.State))
            return Result.Invalid(new ValidationError("Event state is finished already"));
        
        evt.State = newState;
        
        return Result.Success();
    }
}