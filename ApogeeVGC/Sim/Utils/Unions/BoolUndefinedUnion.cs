namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// bool | undefined
/// </summary>
public abstract record BoolUndefinedUnion
{
    public abstract bool IsTruthy();
    public static BoolUndefinedUnion FromBool(bool value) => new BoolBoolUndefinedUnion(value);
    public static BoolUndefinedUnion FromUndefined() => new UndefinedBoolUndefinedUnion(new Undefined());
    public static implicit operator BoolUndefinedUnion(bool value) => new BoolBoolUndefinedUnion(value);
    public static implicit operator BoolUndefinedUnion(Undefined value) =>
      new UndefinedBoolUndefinedUnion(value);
}

public record BoolBoolUndefinedUnion(bool Value) : BoolUndefinedUnion
{
    public override bool IsTruthy() => Value;
}

public record UndefinedBoolUndefinedUnion(Undefined Value) : BoolUndefinedUnion
{
    public override bool IsTruthy() => false;
}
