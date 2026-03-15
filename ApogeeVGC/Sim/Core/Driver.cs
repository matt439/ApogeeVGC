using ApogeeVGC.Data;
using ApogeeVGC.Sim.FormatClasses;

namespace ApogeeVGC.Sim.Core;

public partial class Driver
{
    /// <summary>
    /// When false, skips all Console.ReadLine() pauses so the process exits automatically.
    /// </summary>
    private const bool WaitForInput = true;

    private Library Library { get; } = new();

    // Seeds for random team generation (shared between evaluation and interactive)
    private const int Team1EvalSeed = 54321;
    private const int Team2EvalSeed = 67890;

    // !! EDIT THIS to change the format for all driver modes (except DeterministicRegressionTest) !!
    private const FormatId ActiveFormatId = FormatId.Gen9VgcRegulationI;

    // Model paths derived from the active regulation
    private static string MctsModelPath => $"Tools/DLModel/models/{GetRegulationName(ActiveFormatId)}/battle_model.onnx";
    private static string MctsVocabPath => $"Tools/DLModel/models/{GetRegulationName(ActiveFormatId)}/battle_model_vocab.json";
    private static string MctsTeamPreviewModelPath => $"Tools/DLModel/models/{GetRegulationName(ActiveFormatId)}/team_preview_model.onnx";

    private static string GetRegulationName(FormatId formatId) => formatId switch
    {
        FormatId.Gen9VgcRegulationG => "gen9vgc2024regg",
        FormatId.Gen9VgcRegulationH => "gen9vgc2024regh",
        FormatId.Gen9VgcRegulationI => "gen9vgc2025regi",
        FormatId.Gen9VgcMega => "gen9vgcmega",
        _ => throw new NotSupportedException($"No model mapping for format: {formatId}"),
    };

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
            case DriverMode.MctsDlSingleBattleDebug:
                RunMctsDlSingleBattleDebug(ActiveFormatId);
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
