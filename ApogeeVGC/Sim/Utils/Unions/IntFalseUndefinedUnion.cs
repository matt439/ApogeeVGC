namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// int | false | undefined
/// </summary>
public abstract record IntFalseUndefinedUnion
{
    public static IntFalseUndefinedUnion FromInt(int value) => new IntIntFalseUndefined(value);
    public static IntFalseUndefinedUnion FromFalse() => new FalseIntFalseUndefined();
    public static IntFalseUndefinedUnion FromUndefined() => new UndefinedIntFalseUndefined(new Undefined());
    public static implicit operator IntFalseUndefinedUnion(int value) => new IntIntFalseUndefined(value);
    public static implicit operator IntFalseUndefinedUnion(Undefined value) =>
        new UndefinedIntFalseUndefined(value);
}

public record IntIntFalseUndefined(int Value) : IntFalseUndefinedUnion;
public record FalseIntFalseUndefined : IntFalseUndefinedUnion;
public record UndefinedIntFalseUndefined(Undefined Value) : IntFalseUndefinedUnion;
