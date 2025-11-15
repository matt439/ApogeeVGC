namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// int | int[]
/// </summary>
public abstract record IntIntArrayUnion
{
    public static implicit operator IntIntArrayUnion(int value) => new IntIntIntArrayUnion(value);
 public static implicit operator IntIntArrayUnion(int[] values) => new IntArrayIntIntArrayUnion(values);
}

public record IntIntIntArrayUnion(int Value) : IntIntArrayUnion;
public record IntArrayIntIntArrayUnion(int[] Values) : IntIntArrayUnion;
