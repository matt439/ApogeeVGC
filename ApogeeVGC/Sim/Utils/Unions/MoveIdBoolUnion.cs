using ApogeeVGC.Sim.Moves;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// MoveId | bool
/// </summary>
public abstract record MoveIdBoolUnion
{
    public abstract bool IsTrue();
    public static implicit operator MoveIdBoolUnion(MoveId moveId) => new MoveIdMoveIdBoolUnion(moveId);
    public static implicit operator MoveIdBoolUnion(bool value) => new BoolMoveIdBoolUnion(value);
}

public record MoveIdMoveIdBoolUnion(MoveId MoveId) : MoveIdBoolUnion
{
    public override bool IsTrue() => MoveId != MoveId.None;
}

public record BoolMoveIdBoolUnion(bool Value) : MoveIdBoolUnion
{
    public override bool IsTrue() => Value;
}
