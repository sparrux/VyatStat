using Hub.Domain.Common;

namespace Hub.Domain.ValueObjects;

public sealed class GoalState : ValueObject
{
    GoalState() { }

    public GoalState(int current, int target)
    {
        CurrentValue = current;
        TargetValue = target;
    }
    
    public int CurrentValue { get; private set; }
    public int TargetValue { get; private set; }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CurrentValue;
        yield return TargetValue;
    }
}