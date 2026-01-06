namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// int | false
/// </summary>
public abstract record IntFalseUnion
{
    public abstract int ToInt();
    public abstract IntUndefinedFalseUnion ToIntUndefinedFalseUnion();

    public static IntFalseUnion FromInt(int value) => new IntIntFalseUnion(value);
    public static IntFalseUnion FromFalse() => new FalseIntFalseUnion();

    public static implicit operator IntFalseUnion(int value) => new IntIntFalseUnion(value);

/// <summary>
    /// Compares this IntFalseUnion to another.
    /// False is treated as having the lowest priority (comes last when sorting ascending).
    /// When both are integers, standard integer comparison is used.
    /// </summary>
    public int CompareTo(IntFalseUnion? other)
    {
    if (other == null) return 1;

        return (this, other) switch
        {
   // False < any integer (false has lower priority)
          (FalseIntFalseUnion, IntIntFalseUnion) => 1,  // this is false, other is int -> this > other
         (IntIntFalseUnion, FalseIntFalseUnion) => -1, // this is int, other is false -> this < other

  // Both are false - they're equal
            (FalseIntFalseUnion, FalseIntFalseUnion) => 0,

 // Both are integers - compare the values
    (IntIntFalseUnion thisInt, IntIntFalseUnion otherInt) =>
         thisInt.Value.CompareTo(otherInt.Value),

      _ => 0,
        };
    }
}

public record IntIntFalseUnion(int Value) : IntFalseUnion
{
    public override int ToInt() => Value;
    public override IntUndefinedFalseUnion ToIntUndefinedFalseUnion() => new IntIntUndefinedFalseUnion(Value);
}

public record FalseIntFalseUnion : IntFalseUnion
{
    public override int ToInt() => 0;
    public override IntUndefinedFalseUnion ToIntUndefinedFalseUnion() => new FalseIntUndefinedFalseUnion();
}
