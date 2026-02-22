namespace ApogeeVGC.Sim.Core;

public enum DriverMode
{
    GuiVsRandomSingles,
    GuiVsRandomDoubles,

    ConsoleVsRandomSingles,
    ConsoleVsRandomDoubles,

    RandomVsRandomSinglesEvaluation,
    RandomVsRandomDoublesEvaluation,

    RndVsRndVgcRegIEvaluation,

    /// <summary>
    /// Runs a single VGC Reg I battle with 5 specific seeds for debugging purposes.
    /// Useful for reproducing failures from RndVsRndVgcRegIEvaluation.
    /// </summary>
    SingleBattleDebugVgcRegI,
}
