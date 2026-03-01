using ApogeeVGC.Data;
using ApogeeVGC.Sim.FormatClasses;

namespace ApogeeVGC.Sim.Core;

public partial class Driver
{
    private Library Library { get; } = new();

    // Seeds for interactive test runs — can be changed freely
    private const int PlayerRandom2Seed = 1826;
    private const int BattleSeed = 9885;

    // Don't change these — used for evaluation tests to reproduce errors
    private const int PlayerRandom1EvalSeed = 12345;
    private const int PlayerRandom2EvalSeed = 1818;
    private const int BattleEvalSeed = 9876;

    // Seeds for random team generation (evaluation and interactive)
    private const int Team1EvalSeed = 54321;
    private const int Team2EvalSeed = 67890;

    // Random vs Random evaluation settings
    private const int RandomEvaluationNumTest = 20000;
    private const int NumThreads = 16;

    // MCTS evaluation settings
    private const int MctsEvaluationNumTest = 1000;
    private const int MctsNumThreads = 4;
    private const string MctsModelPath = "Tools/DLModel/battle_model.onnx";
    private const string MctsVocabPath = "Tools/DLModel/vocab.json";

    // !! EDIT THIS to change the format for Console/GUI interactive modes !!
    private const FormatId InteractiveFormatId = FormatId.Gen9VgcRegulationI;

    public void Start(DriverMode mode)
    {
        switch (mode)
        {
            case DriverMode.ConsoleVsRandom:
                RunConsoleVsRandom(InteractiveFormatId);
                break;
            case DriverMode.GuiVsRandom:
                RunGuiVsRandom(InteractiveFormatId);
                break;
            case DriverMode.RndVsRndVgcRegIEvaluation:
                RunRndVsRndVgcRegIEvaluation();
                break;
            case DriverMode.RndVsRndMegaEvaluation:
                RunRndVsRndMegaEvaluation();
                break;
            case DriverMode.MctsVsRndVgcRegIEvaluation:
                RunMctsVsRndVgcRegIEvaluation();
                break;
            case DriverMode.MctsVsRndMegaEvaluation:
                RunMctsVsRndMegaEvaluation();
                break;
            case DriverMode.DeterministicRegressionTest:
                RunDeterministicRegressionTest();
                break;
            case DriverMode.SingleBattleDebugVgcRegI:
                RunSingleBattleDebugVgcRegI();
                break;
            case DriverMode.SingleBattleDebugMega:
                RunSingleBattleDebugMega();
                break;
            default:
                throw new InvalidOperationException($"Driver mode {mode} is not implemented.");
        }
    }
}
