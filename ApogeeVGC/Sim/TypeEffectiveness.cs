using ApogeeVGC.Data;

namespace ApogeeVGC.Sim;

public enum TypeEffectiveness
{
    Normal,
    SuperEffective,
    NotVeryEffective,
    Immune,
}

public static class TypeEffectivenessTools
{
    public static MoveEffectiveness ConvertToMoveEffectiveness(this TypeEffectiveness effectiveness)
    {
        return effectiveness switch
        {
            TypeEffectiveness.Normal => MoveEffectiveness.Normal,
            TypeEffectiveness.SuperEffective => MoveEffectiveness.SuperEffective2X,
            TypeEffectiveness.NotVeryEffective => MoveEffectiveness.NotVeryEffective05X,
            TypeEffectiveness.Immune => MoveEffectiveness.Immune,
            _ => throw new ArgumentException("Invalid type effectiveness value.", nameof(effectiveness))
        };
    }
}