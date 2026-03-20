using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Mcts;

/// <summary>
/// Common interface for MCTS search variants.
/// </summary>
public interface IMctsSearch
{
    (LegalAction ActionA, LegalAction? ActionB) Search(
        Battle battle, SideId sideId, LegalActionSet legalActions);
}
