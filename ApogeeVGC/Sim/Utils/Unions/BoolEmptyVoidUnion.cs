namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// bool | empty | void
/// </summary>
public abstract record BoolEmptyVoidUnion
{
    public static BoolEmptyVoidUnion FromBool(bool value) => new BoolBoolEmptyVoidUnion(value);
    public static BoolEmptyVoidUnion FromEmpty() => new EmptyBoolEmptyVoidUnion(new Empty());
    public static BoolEmptyVoidUnion FromVoid() => new VoidUnionBoolEmptyVoidUnion(new VoidReturn());
    public static implicit operator BoolEmptyVoidUnion(bool value) => new BoolBoolEmptyVoidUnion(value);
    public static implicit operator BoolEmptyVoidUnion(VoidReturn value) => new VoidUnionBoolEmptyVoidUnion(value);
    public static implicit operator BoolEmptyVoidUnion(Empty value) => new EmptyBoolEmptyVoidUnion(value);
}

public record BoolBoolEmptyVoidUnion(bool Value) : BoolEmptyVoidUnion;
public record EmptyBoolEmptyVoidUnion(Empty Value) : BoolEmptyVoidUnion;
public record VoidUnionBoolEmptyVoidUnion(VoidReturn Value) : BoolEmptyVoidUnion;
