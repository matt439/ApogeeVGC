namespace ApogeeVGC.Sim.Core;

public enum DriverMode
{
    GuiVsRandomSingles,
    GuiVsRandomDoubles,

    ConsoleVsRandomSingles,
    ConsoleVsRandomDoubles,

    //RandomVsRandomSingles,
    //RandomVsRandomDoubles,

    RandomVsRandomSinglesEvaluation,
    RandomVsRandomDoublesEvaluation,

    RndVsRndVgcRegIEvaluation,

    /// <summary>
    /// Incrementally tests elements (items, moves, abilities, species) to identify bugs.
    /// Starts with a baseline and adds one element at a time, running verification battles.
    /// </summary>
    IncrementalDebug,

    /// <summary>
    /// Runs a single battle with specific seeds for debugging purposes.
    /// Useful for reproducing specific timeout or bug scenarios.
    /// </summary>
    SingleBattleDebug,
}
