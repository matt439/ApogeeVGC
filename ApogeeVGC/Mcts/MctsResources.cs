using ApogeeVGC.Data;

namespace ApogeeVGC.Mcts;

/// <summary>
/// Static holder for shared MCTS resources (ONNX model, vocab, encoder).
/// Initialize once before creating MCTS players. Thread-safe after initialization.
/// </summary>
public static class MctsResources
{
    private static ModelInference? _model;
    private static TeamPreviewInference? _teamPreviewModel;
    private static Vocab? _vocab;
    private static StateEncoder? _encoder;
    private static Library? _library;

    public static bool IsInitialized { get; private set; }

    public static ModelInference Model => _model
                                          ?? throw new InvalidOperationException(
                                              "MctsResources not initialized. Call Initialize() first.");

    /// <summary>
    /// Team preview model, or null if no team preview ONNX file was provided.
    /// </summary>
    public static TeamPreviewInference? TeamPreviewModel => _teamPreviewModel;

    public static Vocab Vocab => _vocab
                                 ?? throw new InvalidOperationException(
                                     "MctsResources not initialized. Call Initialize() first.");

    public static StateEncoder Encoder => _encoder
                                          ?? throw new InvalidOperationException(
                                              "MctsResources not initialized. Call Initialize() first.");

    public static Library Library => _library
                                     ?? throw new InvalidOperationException(
                                         "MctsResources not initialized. Call Initialize() first.");

    public static MctsConfig Config { get; private set; } = new();

    /// <summary>
    /// Initialize MCTS resources. Call once before creating any MCTS players.
    /// </summary>
    /// <param name="modelPath">Path to the battle_model.onnx file.</param>
    /// <param name="vocabPath">Path to the battle_model_vocab.json file.</param>
    /// <param name="library">Game data library for species/move lookups.</param>
    /// <param name="config">Optional MCTS configuration. Uses defaults if null.</param>
    /// <param name="teamPreviewModelPath">Optional path to team_preview_model.onnx. If null or missing, team preview falls back to random.</param>
    public static void Initialize(string modelPath, string vocabPath, Library library, MctsConfig? config = null,
        string? teamPreviewModelPath = null)
    {
        _library = library;
        _vocab = Vocab.Load(vocabPath, library);
        _encoder = new StateEncoder(_vocab);
        _model = new ModelInference(modelPath, _encoder);
        Config = config ?? new MctsConfig();

        if (teamPreviewModelPath != null && File.Exists(teamPreviewModelPath))
        {
            _teamPreviewModel = new TeamPreviewInference(teamPreviewModelPath, _vocab);
        }

        IsInitialized = true;
    }

    /// <summary>
    /// Dispose of MCTS resources. Call when shutting down.
    /// </summary>
    public static void Shutdown()
    {
        _model?.Dispose();
        _model = null;
        _teamPreviewModel?.Dispose();
        _teamPreviewModel = null;
        _vocab = null;
        _encoder = null;
        _library = null;
        IsInitialized = false;
    }
}