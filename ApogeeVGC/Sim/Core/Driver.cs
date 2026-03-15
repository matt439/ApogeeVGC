using ApogeeVGC.Data;
using ApogeeVGC.Sim.FormatClasses;

namespace ApogeeVGC.Sim.Core;

public partial class Driver
{
    private Library Library { get; } = new();

    // Seeds for random team generation (shared between evaluation and interactive)
    private const int Team1EvalSeed = 54321;
    private const int Team2EvalSeed = 67890;

    // Shared model paths (used by evaluation and ShowdownAssist)
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
