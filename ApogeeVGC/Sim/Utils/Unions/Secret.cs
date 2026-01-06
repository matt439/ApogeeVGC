using ApogeeVGC.Sim.Conditions;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// string | ConditionId
/// </summary>
public abstract record Secret
{
  public static implicit operator Secret(string value) => new SecretString(value);
    public static implicit operator Secret(ConditionId id) => new SecretConditionId(id);
}

public record SecretString(string Value) : Secret
{
    public override string ToString() => Value;
}

public record SecretConditionId(ConditionId Value) : Secret
{
    public override string ToString() => Value.ToString();
}
