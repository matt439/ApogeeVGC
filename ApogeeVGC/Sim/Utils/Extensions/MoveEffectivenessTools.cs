using ApogeeVGC.Data;

namespace ApogeeVGC.Sim.Utils.Extensions;

public static class MoveEffectivenessTools
{
    public static double GetMultiplier(this MoveEffectiveness moveEffectiveness)
    {
        return moveEffectiveness switch
        {
            MoveEffectiveness.Normal => 1.0,
            MoveEffectiveness.SuperEffective2X => 2.0,
            MoveEffectiveness.SuperEffective4X => 4.0,
            MoveEffectiveness.NotVeryEffective05X => 0.5,
            MoveEffectiveness.NotVeryEffective025X => 0.25,
            MoveEffectiveness.Immune => 0.0,
            _ => throw new ArgumentOutOfRangeException(nameof(moveEffectiveness),
                "Invalid move effectiveness value."),
        };
    }
}