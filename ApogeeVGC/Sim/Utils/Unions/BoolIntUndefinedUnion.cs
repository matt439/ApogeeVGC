namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// bool | int | undefined
/// </summary>
public abstract record BoolIntUndefinedUnion
{
    public abstract bool IsTruthy();
    public abstract bool IsZero();
    public abstract int ToInt();

    public static BoolIntUndefinedUnion FromBool(bool value) => new BoolBoolIntUndefinedUnion(value);
    public static BoolIntUndefinedUnion FromInt(int value) => new IntBoolIntUndefinedUnion(value);
    public static BoolIntUndefinedUnion FromUndefined() =>
        new UndefinedBoolIntUndefinedUnion(new Undefined());
    public static implicit operator BoolIntUndefinedUnion(bool value) => new BoolBoolIntUndefinedUnion(value);
    public static implicit operator BoolIntUndefinedUnion(int value) => new IntBoolIntUndefinedUnion(value);
    public static implicit operator BoolIntUndefinedUnion(Undefined value) =>
     new UndefinedBoolIntUndefinedUnion(value);

    public IntFalseUndefinedUnion ToIntFalseUndefinedUnion()
    {
        return this switch
        {
     BoolBoolIntUndefinedUnion boolCase => boolCase.Value
  ? throw new InvalidOperationException("Cannot convert true to IntFalseUndefinedUnion")
   : new FalseIntFalseUndefined(),
            IntBoolIntUndefinedUnion intCase => new IntIntFalseUndefined(intCase.Value),
            UndefinedBoolIntUndefinedUnion => new UndefinedIntFalseUndefined(new Undefined()),
   _ => throw new InvalidOperationException("Unknown BoolIntUndefinedUnion type"),
        };
    }
}

public record BoolBoolIntUndefinedUnion(bool Value) : BoolIntUndefinedUnion
{
    public override bool IsTruthy() => Value;
    public override bool IsZero() => !Value;
    public override int ToInt() => Value ? 1 : 0;
}

public record IntBoolIntUndefinedUnion(int Value) : BoolIntUndefinedUnion
{
    public override bool IsTruthy() => Value != 0;
    public override bool IsZero() => Value == 0;
    public override int ToInt() => Value;
}

public record UndefinedBoolIntUndefinedUnion(Undefined Value) : BoolIntUndefinedUnion
{
    public override bool IsTruthy() => false;
    public override bool IsZero() => false;
    public override int ToInt() =>
    throw new InvalidOperationException(
        "Cannot convert Undefined to int. Undefined and 0 are semantically different. " +
    "Check for Undefined before calling ToInt().");
}
