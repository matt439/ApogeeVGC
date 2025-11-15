using ApogeeVGC.Sim.Moves;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// MoveType | false
/// </summary>
public abstract record MoveTypeFalseUnion
{
    public static implicit operator MoveTypeFalseUnion(MoveType moveType) =>
        new MoveTypeMoveTypeFalseUnion(moveType);
    public static MoveTypeFalseUnion FromFalse() => new FalseMoveTypeFalseUnion();
}

public record MoveTypeMoveTypeFalseUnion(MoveType MoveType) : MoveTypeFalseUnion;
public record FalseMoveTypeFalseUnion : MoveTypeFalseUnion;
