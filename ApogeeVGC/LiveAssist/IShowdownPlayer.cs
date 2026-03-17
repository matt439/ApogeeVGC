using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.BattleClasses;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// Decision-making strategy for a Showdown battle.
/// Analogous to IPlayer for the internal sim, but decoupled from
/// IBattleController/SideId and working with ShowdownState outputs.
/// </summary>
public interface IShowdownPlayer
{
    string Name { get; }

    /// <summary>
    /// Choose actions for the current turn given the legal action set and perspective.
    /// </summary>
    (LegalAction BestA, LegalAction? BestB) ChooseBattle(
        BattlePerspective perspective,
        LegalActionSet actions,
        bool[] maskA,
        bool[] maskB);
}
