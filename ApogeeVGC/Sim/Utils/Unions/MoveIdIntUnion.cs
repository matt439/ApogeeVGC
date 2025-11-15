using ApogeeVGC.Sim.Moves;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// MoveId | int
/// </summary>
public abstract record MoveIdIntUnion
{
    public static implicit operator MoveIdIntUnion(MoveId moveId) => new MoveIdMoveIdIntUnion(moveId);
    public static implicit operator MoveIdIntUnion(int value) => new IntMoveIdIntUnion(value);
}

public record MoveIdMoveIdIntUnion(MoveId MoveId) : MoveIdIntUnion;
public record IntMoveIdIntUnion(int Value) : MoveIdIntUnion;
