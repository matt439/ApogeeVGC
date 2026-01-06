namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// bool | int | empty | void
/// </summary>
public abstract record BoolIntEmptyVoidUnion
{
    public static BoolIntEmptyVoidUnion FromBool(bool value) => new BoolBoolIntEmptyVoidUnion(value);
    public static BoolIntEmptyVoidUnion FromInt(int value) => new IntBoolIntEmptyVoidUnion(value);
    public static BoolIntEmptyVoidUnion FromEmpty() => new EmptyBoolIntEmptyVoidUnion(new Empty());
    public static BoolIntEmptyVoidUnion FromVoid() => new VoidUnionBoolIntEmptyVoidUnion(new VoidReturn());
    public static implicit operator BoolIntEmptyVoidUnion(bool value) => new BoolBoolIntEmptyVoidUnion(value);
    public static implicit operator BoolIntEmptyVoidUnion(int value) => new IntBoolIntEmptyVoidUnion(value);
public static implicit operator BoolIntEmptyVoidUnion(VoidReturn value) =>
        new VoidUnionBoolIntEmptyVoidUnion(value);
    public static implicit operator BoolIntEmptyVoidUnion(Empty value) =>
     new EmptyBoolIntEmptyVoidUnion(value);
}

public record BoolBoolIntEmptyVoidUnion(bool Value) : BoolIntEmptyVoidUnion;
public record IntBoolIntEmptyVoidUnion(int Value) : BoolIntEmptyVoidUnion;
public record EmptyBoolIntEmptyVoidUnion(Empty Value) : BoolIntEmptyVoidUnion;
public record VoidUnionBoolIntEmptyVoidUnion(VoidReturn Value) : BoolIntEmptyVoidUnion;
