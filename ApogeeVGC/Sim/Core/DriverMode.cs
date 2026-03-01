namespace ApogeeVGC.Sim.Core;

public enum DriverMode
{
    GuiVsRandomSingles,
    GuiVsRandomDoubles,

    ConsoleVsRandomSingles,
    ConsoleVsRandomDoubles,

    RndVsRndVgcRegIEvaluation,
    RndVsRndMegaEvaluation,

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
