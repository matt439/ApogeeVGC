using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Moves;

namespace ApogeeVGC.Sim.Utils.Extensions;

public static class MoveIdTools
{
    /// <summary>
    /// Converts a MoveId to its corresponding ConditionId for moves that have associated conditions.
    /// This is used for moves that create conditions with matching names (e.g., Light Screen, Protect, Trick Room).
    /// </summary>
    /// <param name="moveId">The MoveId to convert</param>
    /// <returns>The corresponding ConditionId</returns>
    /// <exception cref="ArgumentException">Thrown when the MoveId does not have a corresponding ConditionId</exception>
    public static ConditionId ToConditionId(this MoveId moveId)
    {
        return moveId switch
        {
            MoveId.LeechSeed => ConditionId.LeechSeed,
            MoveId.TrickRoom => ConditionId.TrickRoom,
            MoveId.Protect => ConditionId.Protect,
            MoveId.Tailwind => ConditionId.Tailwind,
            MoveId.Reflect => ConditionId.Reflect,
            MoveId.LightScreen => ConditionId.LightScreen,
            MoveId.Yawn => ConditionId.Yawn,
            MoveId.RevivalBlessing => ConditionId.RevivalBlessing,
            MoveId.GigatonHammer => ConditionId.GigatonHammer,
            MoveId.BloodMoon => ConditionId.BloodMoon,
            _ => throw new ArgumentException($"MoveId '{moveId}' does not have a corresponding ConditionId.",
                nameof(moveId)),
        };
    }
}