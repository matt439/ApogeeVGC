namespace ApogeeVGC.Sim.Core;

public enum DriverMode
{
    // Interactive modes
    ConsoleVsRandom,
    GuiVsRandom,

    // Parallel random-team evaluation
    RndVsRndEvaluation,

    // MCTS vs Random evaluation
    MctsVsRndEvaluation,

    // Greedy vs Random evaluation
    GreedyVsRndEvaluation,

    // Deterministic regression test — runs fixed battles across formats, prints hash
    DeterministicRegressionTest,

    /// <summary>
    /// Runs a single battle with 5 specific seeds for debugging purposes.
    /// Useful for reproducing failures from RndVsRndEvaluation.
    /// </summary>
    SingleBattleDebug,
}
