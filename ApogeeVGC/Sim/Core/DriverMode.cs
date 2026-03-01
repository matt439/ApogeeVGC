namespace ApogeeVGC.Sim.Core;

public enum DriverMode
{
    // Interactive modes — format controlled by InteractiveFormatId constant in Driver.cs
    ConsoleVsRandom,
    GuiVsRandom,

    // Parallel random-team evaluations
    RndVsRndVgcRegIEvaluation,
    RndVsRndMegaEvaluation,

    // MCTS vs Random evaluations
    MctsVsRndVgcRegIEvaluation,
    MctsVsRndMegaEvaluation,

    // Deterministic regression test — runs fixed battles across formats, prints hash
    DeterministicRegressionTest,

    /// <summary>
    /// Runs a single VGC Reg I battle with 5 specific seeds for debugging purposes.
    /// Useful for reproducing failures from RndVsRndVgcRegIEvaluation.
    /// </summary>
    SingleBattleDebugVgcRegI,

    /// <summary>
    /// Runs a single Mega Evolution battle with 5 specific seeds for debugging purposes.
    /// Useful for reproducing failures from RndVsRndMegaEvaluation.
    /// </summary>
    SingleBattleDebugMega,
}
