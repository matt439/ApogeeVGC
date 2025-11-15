using ApogeeVGC.Sim.Moves;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// MoveId | void
/// </summary>
public abstract record MoveIdVoidUnion
{
    public static implicit operator MoveIdVoidUnion(MoveId moveId) => new MoveIdMoveIdVoidUnion(moveId);
    public static implicit operator MoveIdVoidUnion(VoidReturn value) => new VoidMoveIdVoidUnion(value);
    public static MoveIdVoidUnion FromVoid() => new VoidMoveIdVoidUnion(new VoidReturn());
}

public record MoveIdMoveIdVoidUnion(MoveId MoveId) : MoveIdVoidUnion;
public record VoidMoveIdVoidUnion(VoidReturn Value) : MoveIdVoidUnion;
