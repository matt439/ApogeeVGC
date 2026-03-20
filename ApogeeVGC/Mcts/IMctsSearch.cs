using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Mcts;

/// <summary>
/// Common interface for MCTS search variants.
/// </summary>
public interface IMctsSearch
{
    /// <summary>
    /// Run MCTS search. Uses fixed iterations from MctsConfig by default.
    /// Pass a cancellation token for time-based termination (e.g., Showdown turn timer).
    /// When a token is provided, runs iterations until cancelled instead of a fixed count.
    /// </summary>
    (LegalAction ActionA, LegalAction? ActionB) Search(
        Battle battle, SideId sideId, LegalActionSet legalActions,
        CancellationToken cancellationToken = default);
}
