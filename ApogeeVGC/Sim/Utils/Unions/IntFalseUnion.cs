namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// int | false — a struct-based discriminated union that avoids heap allocations.
/// Replaces the previous IntFalseUnion / IntIntFalseUnion / FalseIntFalseUnion class hierarchy.
/// </summary>
public readonly record struct IntFalseUnion
{
    public int Value { get; }
    public bool IsFalse { get; }
    public bool IsInt => !IsFalse;

    private IntFalseUnion(int value, bool isFalse)
    {
        Value = value;
        IsFalse = isFalse;
    }

    public int ToInt() => IsFalse ? 0 : Value;

    public IntUndefinedFalseUnion ToIntUndefinedFalseUnion() =>
        IsFalse
            ? new FalseIntUndefinedFalseUnion()
            : new IntIntUndefinedFalseUnion(Value);

    public static IntFalseUnion FromInt(int value) => new(value, false);
    public static IntFalseUnion FromFalse() => new(0, true);

    public static implicit operator IntFalseUnion(int value) => new(value, false);

    /// <summary>
    /// Compares this IntFalseUnion to another.
    /// False is treated as having the lowest priority (comes last when sorting ascending).
    /// When both are integers, standard integer comparison is used.
    /// </summary>
    public int CompareTo(IntFalseUnion? other)
    {
        if (other == null) return 1;
        var o = other.Value;

        return (IsFalse, o.IsFalse) switch
        {
            (true, false) => 1,    // this is false, other is int -> this > other
            (false, true) => -1,   // this is int, other is false -> this < other
            (true, true) => 0,     // both false
            (false, false) => Value.CompareTo(o.Value),
        };
    }
}
