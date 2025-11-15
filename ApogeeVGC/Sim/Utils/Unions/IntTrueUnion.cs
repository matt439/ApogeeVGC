namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// int | true
/// </summary>
public abstract record IntTrueUnion
{
    public static IntTrueUnion FromInt(int value) => new IntIntTrueUnion(value);
    public static IntTrueUnion FromTrue() => new TrueIntTrueUnion();
    public static implicit operator IntTrueUnion(int value) => new IntIntTrueUnion(value);
}

public record IntIntTrueUnion(int Value) : IntTrueUnion;
public record TrueIntTrueUnion : IntTrueUnion;
