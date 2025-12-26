namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// bool| 0
/// </summary>
public abstract record BoolZeroUnion
{
    public static implicit operator BoolZeroUnion(bool value) => new BoolBoolZeroUnion(value);
    public static BoolZeroUnion FromZero() => new ZeroBoolZeroUnion();
    public abstract bool IsTruthy();
}

public record BoolBoolZeroUnion(bool Value) : BoolZeroUnion
{
    public override bool IsTruthy() => Value;
}

public record ZeroBoolZeroUnion : BoolZeroUnion
{
    public override bool IsTruthy() => false;
}