namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// int | bool | void
/// </summary>
public abstract record IntBoolVoidUnion
{
    public static IntBoolVoidUnion FromInt(int value) => new IntIntBoolVoidUnion(value);
    public static IntBoolVoidUnion FromBool(bool value) => new BoolIntBoolVoidUnion(value);
    public static IntBoolVoidUnion FromVoid() => new VoidIntBoolVoidUnion(new VoidReturn());
    public static implicit operator IntBoolVoidUnion(int value) => new IntIntBoolVoidUnion(value);
 public static implicit operator IntBoolVoidUnion(bool value) => new BoolIntBoolVoidUnion(value);
    public static implicit operator IntBoolVoidUnion(VoidReturn value) => new VoidIntBoolVoidUnion(value);
}

public record IntIntBoolVoidUnion(int Value) : IntBoolVoidUnion;
public record BoolIntBoolVoidUnion(bool Value) : IntBoolVoidUnion;
public record VoidIntBoolVoidUnion(VoidReturn Value) : IntBoolVoidUnion;
