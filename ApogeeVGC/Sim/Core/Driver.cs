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
    private const int RandomEvaluationNumTest = 100000;
    private const int NumThreads = 16;

    // MCTS evaluation settings
    private const int MctsEvaluationNumTest = 1000;
    private const int MctsNumThreads = 4;
    private const string MctsModelPath = "Tools/DLModel/battle_model.onnx";
    private const string MctsVocabPath = "Tools/DLModel/vocab.json";

    // !! EDIT THIS to change the format for all driver modes (except DeterministicRegressionTest) !!
    private const FormatId ActiveFormatId = FormatId.Gen9VgcRegulationI;

    public void Start(DriverMode mode)
    {
        switch (mode)
        {
            case DriverMode.ConsoleVsRandom:
                RunConsoleVsRandom(ActiveFormatId);
                break;
            case DriverMode.GuiVsRandom:
                RunGuiVsRandom(ActiveFormatId);
                break;
            case DriverMode.RndVsRndEvaluation:
                RunRandomTeamEvaluation(ActiveFormatId);
                break;
            case DriverMode.MctsVsRndEvaluation:
                RunMctsVsRandomEvaluation(ActiveFormatId);
                break;
            case DriverMode.DeterministicRegressionTest:
                RunDeterministicRegressionTest();
                break;
            case DriverMode.SingleBattleDebug:
                RunSingleBattleDebug(ActiveFormatId);
                break;
            default:
                throw new InvalidOperationException($"Driver mode {mode} is not implemented.");
        }
    }
}
