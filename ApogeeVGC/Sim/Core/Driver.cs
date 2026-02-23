using ApogeeVGC.Data;

namespace ApogeeVGC.Sim.Core;

public partial class Driver
{
    private Library Library { get; } = new();

    // Seeds for standard test runs - can be changed freely
    //private const int PlayerRandom1Seed = 12352;
    private const int PlayerRandom2Seed = 1826;
    private const int BattleSeed = 9885;

    // Don't change these - used for evaluation test to reproduce errors
    private const int PlayerRandom1EvalSeed = 12345;
    private const int PlayerRandom2EvalSeed = 1818;
    private const int BattleEvalSeed = 9876;

    // Seeds for VGC Regulation I random team generation evaluation
    private const int Team1EvalSeed = 54321;
    private const int Team2EvalSeed = 67890;

    private const int RandomEvaluationNumTest = 30000;
    private const int NumThreads = 16;
    private const int BattleTimeoutMilliseconds = 10000; // 3 seconds timeout per battle

    public void Start(DriverMode mode)
    {
        switch (mode)
        {
            case DriverMode.GuiVsRandomSingles:
                RunGuiVsRandomSinglesTest();
                break;
            case DriverMode.GuiVsRandomDoubles:
                RunGuiVsRandomDoublesTest();
                break;
            case DriverMode.ConsoleVsRandomSingles:
                RunConsoleVsRandomSinglesTest();
                break;
            case DriverMode.ConsoleVsRandomDoubles:
                RunConsoleVsRandomDoublesTest();
                break;
            case DriverMode.RandomVsRandomSinglesEvaluation:
                RunRandomVsRandomSinglesEvaluationTest();
                break;
            case DriverMode.RandomVsRandomDoublesEvaluation:
                RunRandomVsRandomDoublesEvaluationTest();
                break;
            case DriverMode.RndVsRndVgcRegIEvaluation:
                RunRndVsRndVgcRegIEvaluation();
                break;
            case DriverMode.SingleBattleDebugVgcRegI:
                RunSingleBattleDebugVgcRegI();
                break;
            default:
                throw new InvalidOperationException($"Driver mode {mode} is not implemented.");
        }
    }
}
