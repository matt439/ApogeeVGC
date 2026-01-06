namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// double | void
/// </summary>
public abstract record DoubleVoidUnion
{
    public static DoubleVoidUnion FromDouble(double value) => new DoubleDoubleVoidUnion(value);
    public static DoubleVoidUnion FromVoid() => new VoidDoubleVoidUnion(new VoidReturn());
    public static implicit operator DoubleVoidUnion(double value) => new DoubleDoubleVoidUnion(value);
    public static implicit operator DoubleVoidUnion(VoidReturn value) => new VoidDoubleVoidUnion(value);
}

public record DoubleDoubleVoidUnion(double Value) : DoubleVoidUnion;
public record VoidDoubleVoidUnion(VoidReturn Value) : DoubleVoidUnion;
