namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// int | void
/// </summary>
public abstract record IntVoidUnion
{
    public static IntVoidUnion FromInt(int value) => new IntIntVoidUnion(value);
    public static IntVoidUnion FromVoid() => new VoidIntVoidUnion(new VoidReturn());
 public static implicit operator IntVoidUnion(int value) => new IntIntVoidUnion(value);
    public static implicit operator IntVoidUnion(VoidReturn value) => new VoidIntVoidUnion(value);
}

public record IntIntVoidUnion(int Value) : IntVoidUnion;
public record VoidIntVoidUnion(VoidReturn Value) : IntVoidUnion;
