namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// void | false
/// Used for event handlers that can either:
/// - Return void/nothing (no effect on event flow)
/// - Return false to indicate the event should stop/fail
/// </summary>
public abstract record VoidFalseUnion
{
    public static VoidFalseUnion FromVoid() => new VoidVoidFalseUnion(new VoidReturn());
    public static VoidFalseUnion FromFalse() => new FalseVoidFalseUnion();
    
    public static implicit operator VoidFalseUnion(VoidReturn value) => new VoidVoidFalseUnion(value);
    
    /// <summary>
    /// Returns true if this union represents false (failure/stop signal).
    /// </summary>
    public bool IsFalse => this is FalseVoidFalseUnion;
    
    /// <summary>
    /// Returns true if this union represents void (no effect).
    /// </summary>
    public bool IsVoid => this is VoidVoidFalseUnion;
}

public record VoidVoidFalseUnion(VoidReturn Value) : VoidFalseUnion;
public record FalseVoidFalseUnion : VoidFalseUnion;
