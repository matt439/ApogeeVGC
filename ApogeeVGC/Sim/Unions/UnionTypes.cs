namespace ApogeeVGC.Sim.Unions;

/// <summary>int | bool</summary>
public abstract record IntBoolUnion
{
    public static implicit operator IntBoolUnion(int value) => new IntIntBoolUnion(value);
    public static implicit operator IntBoolUnion(bool value) => new BoolIntBoolUnion(value);
}
public record IntIntBoolUnion(int Value) : IntBoolUnion;
public record BoolIntBoolUnion(bool Value) : IntBoolUnion;


/// <summary>
/// int | false
/// </summary>
public abstract record IntFalseUnion
{
    public static implicit operator IntFalseUnion(int value) => new IntIntFalseUnion(value);

    public static implicit operator IntFalseUnion(bool value) =>
        value ? throw new ArgumentException("must be 'false'") : new FalseIntFalseUnion();
}

public record IntIntFalseUnion(int Value) : IntFalseUnion;
public record FalseIntFalseUnion : IntFalseUnion;