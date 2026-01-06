namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// int | Undefined | false
/// </summary>
public abstract record IntUndefinedFalseUnion
{
    public static implicit operator IntUndefinedFalseUnion(int value) =>
        new IntIntUndefinedFalseUnion(value);
    public static implicit operator IntUndefinedFalseUnion(Undefined value) =>
        new UndefinedIntUndefinedFalseUnion(value);
    public static IntUndefinedFalseUnion FromFalse() => new FalseIntUndefinedFalseUnion();
}

public record IntIntUndefinedFalseUnion(int Value) : IntUndefinedFalseUnion;
public record UndefinedIntUndefinedFalseUnion(Undefined Value) : IntUndefinedFalseUnion;
public record FalseIntUndefinedFalseUnion : IntUndefinedFalseUnion;
