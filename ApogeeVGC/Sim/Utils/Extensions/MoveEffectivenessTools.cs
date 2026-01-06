using ApogeeVGC.Data;

namespace ApogeeVGC.Sim.Utils.Extensions;

public static class MoveEffectivenessTools
{
    /// <summary>
    /// Converts MoveEffectiveness enum to integer modifier for event system calculations.
    /// This matches the TypeScript behavior where values are summed.
    /// </summary>
    public static int ToModifier(this MoveEffectiveness effectiveness)
    {
        return effectiveness switch
        {
            MoveEffectiveness.Normal => 0,
            MoveEffectiveness.SuperEffective2X => 1,
            MoveEffectiveness.NotVeryEffective05X => -1,
            MoveEffectiveness.Immune => 0,  // Handled separately by immunity checks
            MoveEffectiveness.SuperEffective4X => 2,
            MoveEffectiveness.NotVeryEffective025X => -2,
            _ => 0,
        };
    }

    /// <summary>
    /// Converts integer modifier back to MoveEffectiveness enum.
    /// This is used after event calculations to get the final effectiveness.
    /// </summary>
    public static MoveEffectiveness ToMoveEffectiveness(this int modifier)
    {
        return modifier switch
        {
            >= 2 => MoveEffectiveness.SuperEffective4X,     // 4x damage
            1 => MoveEffectiveness.SuperEffective2X,         // 2x damage
            0 => MoveEffectiveness.Normal,                   // 1x damage
            -1 => MoveEffectiveness.NotVeryEffective05X,     // 0.5x damage
            <= -2 => MoveEffectiveness.NotVeryEffective025X, // 0.25x damage
        };
    }

    /// <summary>
    /// Converts MoveEffectiveness enum to actual damage multiplier.
    /// </summary>
    public static double ToMultiplier(this MoveEffectiveness effectiveness)
    {
        return effectiveness switch
        {
            MoveEffectiveness.Normal => 1.0,
            MoveEffectiveness.SuperEffective2X => 2.0,
            MoveEffectiveness.NotVeryEffective05X => 0.5,
            MoveEffectiveness.Immune => 0.0,
            MoveEffectiveness.SuperEffective4X => 4.0,
            MoveEffectiveness.NotVeryEffective025X => 0.25,
            _ => 1.0,
        };
    }

    /// <summary>
    /// Checks if the effectiveness represents immunity.
    /// </summary>
    public static bool IsImmune(this MoveEffectiveness effectiveness)
    {
        return effectiveness == MoveEffectiveness.Immune;
    }

    /// <summary>
    /// Checks if the effectiveness is super effective (2x or 4x).
    /// </summary>
    public static bool IsSuperEffective(this MoveEffectiveness effectiveness)
    {
        return effectiveness is MoveEffectiveness.SuperEffective2X or MoveEffectiveness.SuperEffective4X;
    }

    /// <summary>
    /// Checks if the effectiveness is not very effective (0.5x or 0.25x).
    /// </summary>
    public static bool IsNotVeryEffective(this MoveEffectiveness effectiveness)
    {
        return effectiveness is MoveEffectiveness.NotVeryEffective05X or MoveEffectiveness.NotVeryEffective025X;
    }
}