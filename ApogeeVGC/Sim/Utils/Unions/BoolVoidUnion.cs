namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// bool | void
/// </summary>
public abstract record BoolVoidUnion
{
    public static BoolVoidUnion FromBool(bool value) => new BoolBoolVoidUnion(value);
    public static BoolVoidUnion FromVoid() => new VoidBoolVoidUnion(new VoidReturn());
    public static implicit operator BoolVoidUnion(bool value) => new BoolBoolVoidUnion(value);
    public static implicit operator BoolVoidUnion(VoidReturn value) => new VoidBoolVoidUnion(value);
}

public record BoolBoolVoidUnion(bool Value) : BoolVoidUnion;
public record VoidBoolVoidUnion(VoidReturn Value) : BoolVoidUnion;
