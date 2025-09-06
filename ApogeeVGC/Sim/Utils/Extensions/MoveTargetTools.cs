using System.Diagnostics;
using System.Runtime.InteropServices;
using ApogeeVGC.Sim.Moves;

namespace ApogeeVGC.Sim.Utils.Extensions;

public static class MoveTargetTools
{

    /// <summary>
    /// When locking in a move, there will always be a certain number of targets.
    /// </summary>
    /// <param name="target">The move target type</param>
    /// <returns>The minimum and maximum possible Pokemon targets for a move target type</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static (int, int) GetPossibleTargetMinMax(this MoveTarget target)
    {
        return target switch
        {
            MoveTarget.AdjacentAlly => (1, 1),
            MoveTarget.AdjacentAllyOrSelf => (1, 1),
            MoveTarget.AdjacentFoe => (1, 1),
            MoveTarget.All => (2, 4),
            MoveTarget.AllAdjacent => (1, 3),
            MoveTarget.AllAdjacentFoes => (1, 2),
            MoveTarget.Allies => (1, 2),
            MoveTarget.AllySide => (0, 0),
            MoveTarget.AllyTeam => (1, 2),
            MoveTarget.Any => (1, 1),
            MoveTarget.FoeSide => (0, 0),
            MoveTarget.Normal => (1, 1),
            MoveTarget.RandomNormal => (1, 1),
            MoveTarget.Scripted => (1, 1),
            MoveTarget.Self => (1, 1),
            MoveTarget.None => (0, 0),
            MoveTarget.Field => (0, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };
    }
}