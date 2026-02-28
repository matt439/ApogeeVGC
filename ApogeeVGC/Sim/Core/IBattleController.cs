using ApogeeVGC.Sim.BattleClasses;

namespace ApogeeVGC.Sim.Core;

public interface IBattleController
{
    // void SubmitChoice(SideId sideId, Choice choice);
    BattlePerspective GetCurrentPerspective(SideId sideId);

    /// <summary>
    /// Returns the current Battle instance, used by MCTS for deep copy simulation.
    /// </summary>
    Battle? Battle { get; }
}