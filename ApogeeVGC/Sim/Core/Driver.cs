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
    private const int RandomEvaluationNumTest = 50000;
    private const int NumThreads = 16;

    // Greedy evaluation settings
    private const int GreedyEvaluationNumTest = 200000;
    private const int GreedyNumThreads = 32;

    // MCTS evaluation settings
    private const int MctsEvaluationNumTest = 1000;
    private const int MctsNumThreads = 32;

    // Standalone MCTS evaluation settings
    private const int MctsStandaloneEvaluationNumTest = 100;
    private const int MctsStandaloneNumThreads = 32;
    private const int MctsStandaloneIterations = 1000;

    // MCTS-DL evaluation settings (DL priors + value, no info tracking)
    private const int MctsDlEvaluationNumTest = 100;
    private const int MctsDlNumThreads = 32;
    private const int MctsDlIterations = 1000;

    // DL-Greedy evaluation settings (argmax policy, no search)
    private const int DlGreedyEvaluationNumTest = 1000;
    private const int DlGreedyNumThreads = 32;

    // Shared model paths
    private const string MctsModelPath = "Tools/DLModel/battle_model.onnx";
    private const string MctsVocabPath = "Tools/DLModel/battle_model_vocab.json";
    private const string MctsTeamPreviewModelPath = "Tools/DLModel/team_preview_model.onnx";

    // !! EDIT THIS to change the format for all driver modes (except DeterministicRegressionTest) !!
    private const FormatId ActiveFormatId = FormatId.Gen9VgcRegulationI;

    public void Start(DriverMode mode, string? formatOverride = null)
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
            case DriverMode.GreedyVsRndEvaluation:
                RunGreedyVsRandomEvaluation(ActiveFormatId);
                break;
            case DriverMode.MctsStandaloneVsRndEvaluation:
                RunMctsStandaloneVsRandomEvaluation(ActiveFormatId);
                break;
            case DriverMode.MctsDlVsRndEvaluation:
                RunMctsDLVsRandomEvaluation(ActiveFormatId);
                break;
            case DriverMode.DlGreedyVsRndEvaluation:
                RunDLGreedyVsRandomEvaluation(ActiveFormatId);
                break;
            case DriverMode.ShowdownAssist:
                RunShowdownAssist(ActiveFormatId);
                break;
            case DriverMode.SingleBattleDebug:
                RunSingleBattleDebug(ActiveFormatId);
                break;
            case DriverMode.Gen5RngVerification:
                RunGen5RngVerification();
                break;
            case DriverMode.EquivalenceTest:
                RunEquivalenceTest(
                    "Tools/EquivalenceTest/batch_cache/battle_000000.fixture.json",
                    "Tools/EquivalenceTest/batch_cache/battle_000000.log");
                break;
            default:
                throw new InvalidOperationException($"Driver mode {mode} is not implemented.");
        }
    }
}
