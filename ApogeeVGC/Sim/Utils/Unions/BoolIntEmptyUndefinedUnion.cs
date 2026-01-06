namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// bool | int | empty | undefined
/// </summary>
public abstract record BoolIntEmptyUndefinedUnion
{
    public abstract bool IsTruthy();
    public abstract bool IsZero();

    public abstract BoolIntUndefinedUnion ToBoolIntUndefinedUnion();

    public static BoolIntEmptyUndefinedUnion FromBool(bool value) => new BoolBoolIntEmptyUndefinedUnion(value);
    public static BoolIntEmptyUndefinedUnion FromInt(int value) => new IntBoolIntEmptyUndefinedUnion(value);
    public static BoolIntEmptyUndefinedUnion FromEmpty() => new EmptyBoolIntEmptyUndefinedUnion(new Empty());
    public static BoolIntEmptyUndefinedUnion FromUndefined() =>
        new UndefinedBoolIntEmptyUndefinedUnion(new Undefined());
    public static implicit operator BoolIntEmptyUndefinedUnion(bool value) =>
        new BoolBoolIntEmptyUndefinedUnion(value);
    public static implicit operator BoolIntEmptyUndefinedUnion(int value) =>
        new IntBoolIntEmptyUndefinedUnion(value);
    public static implicit operator BoolIntEmptyUndefinedUnion(Empty value) =>
        new EmptyBoolIntEmptyUndefinedUnion(value);
    public static implicit operator BoolIntEmptyUndefinedUnion(Undefined value) =>
        new UndefinedBoolIntEmptyUndefinedUnion(value);

    public static BoolIntEmptyUndefinedUnion? FromNullableBoolIntUndefinedUnion(BoolIntUndefinedUnion? value)
    {
        return value switch
     {
            BoolBoolIntUndefinedUnion b => b.Value,
         IntBoolIntUndefinedUnion i => i.Value,
       UndefinedBoolIntUndefinedUnion => new Undefined(),
  null => null,
  _ => throw new InvalidOperationException(),
        };
    }
}

public record BoolBoolIntEmptyUndefinedUnion(bool Value) : BoolIntEmptyUndefinedUnion
{
    public override bool IsTruthy() => Value;
    public override bool IsZero() => false; // Boolean false is NOT zero - it's a failure

    public override BoolIntUndefinedUnion ToBoolIntUndefinedUnion() => Value;
}

public record IntBoolIntEmptyUndefinedUnion(int Value) : BoolIntEmptyUndefinedUnion
{
 public override bool IsTruthy() => Value != 0;
    public override bool IsZero() => Value == 0;
    public override BoolIntUndefinedUnion ToBoolIntUndefinedUnion() => Value;
}

public record EmptyBoolIntEmptyUndefinedUnion(Empty Value) : BoolIntEmptyUndefinedUnion
{
    public override bool IsTruthy() => false;
    public override bool IsZero() => false; // Empty (NOT_FAIL) is not zero

    public override BoolIntUndefinedUnion ToBoolIntUndefinedUnion() =>
    throw new InvalidOperationException("BoolIntUndefinedUnion cannot hold the Empty type.");
}

public record UndefinedBoolIntEmptyUndefinedUnion(Undefined Value) : BoolIntEmptyUndefinedUnion
{
    public override bool IsTruthy() => false;
    public override bool IsZero() => false;
    public override BoolIntUndefinedUnion ToBoolIntUndefinedUnion() => new Undefined();
}
