using ApogeeVGC.Sim.Conditions;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// ConditionId | bool
/// </summary>
public abstract record ConditionIdBoolUnion
{
    public static implicit operator ConditionIdBoolUnion(ConditionId conditionId) =>
        new ConditionIdConditionIdBoolUnion(conditionId);
    public static implicit operator ConditionIdBoolUnion(bool value) => new BoolConditionIdBoolUnion(value);
}

public record ConditionIdConditionIdBoolUnion(ConditionId ConditionId) : ConditionIdBoolUnion;
public record BoolConditionIdBoolUnion(bool Value) : ConditionIdBoolUnion;
