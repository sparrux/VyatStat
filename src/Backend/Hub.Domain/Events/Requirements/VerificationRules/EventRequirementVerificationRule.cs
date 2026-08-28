using System.Diagnostics.CodeAnalysis;
using Hub.Domain.Common;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events.Requirements.VerificationRules;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public abstract class EventRequirementVerificationRule : Auditable
{
    [SuppressMessage("ReSharper", "EmptyConstructor")]
    protected EventRequirementVerificationRule() { }

    public Guid VerifierId { get; private set; }
    public EventRequirementRuleVerifier Verifier { get; private set; }
}

public sealed class ContributionPaidVerificationRule : EventRequirementVerificationRule
{
    ContributionPaidVerificationRule() { }
    
    /// <summary>
    /// The special code used to verify the rule execution.
    /// </summary>
    public string? Code { get; private set; }
}