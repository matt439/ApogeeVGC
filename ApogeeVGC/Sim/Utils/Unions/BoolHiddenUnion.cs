namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// bool | 'hidden'
/// </summary>
public abstract record BoolHiddenUnion
{
    public static implicit operator BoolHiddenUnion(bool value) => new BoolBoolHiddenUnion(value);
    public static BoolHiddenUnion FromHidden() => new HiddenBoolHiddenUnion();

    /// <summary>
  /// Returns true if this union represents a truthy value (true or 'hidden').
    /// Used to check if a move is effectively disabled.
    /// </summary>
    public bool IsTruthy() => this switch
    {
    BoolBoolHiddenUnion { Value: true } => true,
        HiddenBoolHiddenUnion => true,
        _ => false,
    };

    /// <summary>
/// Returns true if this union is explicitly the boolean value true.
    /// </summary>
    public bool IsTrue() => this is BoolBoolHiddenUnion { Value: true };
}

public record BoolBoolHiddenUnion(bool Value) : BoolHiddenUnion;
public record HiddenBoolHiddenUnion : BoolHiddenUnion;
