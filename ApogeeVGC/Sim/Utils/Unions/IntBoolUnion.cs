namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// int | bool
/// </summary>
public abstract record IntBoolUnion
{
    public abstract int ToInt();

    public static IntBoolUnion FromInt(int value) => new IntIntBoolUnion(value);
    public static IntBoolUnion FromBool(bool value) => new BoolIntBoolUnion(value);

    public static implicit operator IntBoolUnion(int value) => new IntIntBoolUnion(value);
    public static implicit operator IntBoolUnion(bool value) => new BoolIntBoolUnion(value);
}

public record IntIntBoolUnion(int Value) : IntBoolUnion
{
    public override int ToInt() => Value;
}

public record BoolIntBoolUnion(bool Value) : IntBoolUnion
{
    public override int ToInt() => Value ? 1 : 0;
}
