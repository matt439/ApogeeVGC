namespace ApogeeVGC.Sim.Utils;

/// <summary>
/// int | bool
/// </summary>
public abstract record IntBoolUnion
{
    public static IntBoolUnion FromInt(int value) => new IntIntBoolUnion(value);
    public static IntBoolUnion FromBool(bool value) => new BoolIntBoolUnion(value);

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
    public static IntFalseUnion FromInt(int value) => new IntIntFalseUnion(value);
    public static IntFalseUnion FromFalse() => new FalseIntFalseUnion();

    public static implicit operator IntFalseUnion(int value) => new IntIntFalseUnion(value);
}
public record IntIntFalseUnion(int Value) : IntFalseUnion;
public record FalseIntFalseUnion : IntFalseUnion;