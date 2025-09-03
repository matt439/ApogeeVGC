using ApogeeVGC.Data;
using ApogeeVGC.Sim.Types;

namespace ApogeeVGC.Sim.Utils.Extensions
{
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
}
