namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Delegate | void
/// </summary>
public abstract record DelegateVoidUnion
{
    public static implicit operator DelegateVoidUnion(Delegate del) => new DelegateDelegateVoidUnion(del);
    public static implicit operator DelegateVoidUnion(VoidReturn value) => new VoidDelegateVoidUnion(value);
    public static DelegateVoidUnion FromVoid() => new VoidDelegateVoidUnion(new VoidReturn());
}

public record DelegateDelegateVoidUnion(Delegate Del) : DelegateVoidUnion;
public record VoidDelegateVoidUnion(VoidReturn Value) : DelegateVoidUnion;
