using ApogeeVGC.Data;

namespace ApogeeVGC.Mcts;

/// <summary>
/// Static holder for shared MCTS resources (ONNX model, vocab, encoder).
/// Initialize once before creating MCTS players. Thread-safe after initialization.
/// </summary>
public static class MctsResources
{
    private static ModelInference? _model;
    private static Vocab? _vocab;
    private static StateEncoder? _encoder;
    private static MctsConfig _config = new();
    private static bool _initialized;

    public static bool IsInitialized => _initialized;

    public static ModelInference Model => _model
        ?? throw new InvalidOperationException("MctsResources not initialized. Call Initialize() first.");

    public static Vocab Vocab => _vocab
        ?? throw new InvalidOperationException("MctsResources not initialized. Call Initialize() first.");

    public static StateEncoder Encoder => _encoder
        ?? throw new InvalidOperationException("MctsResources not initialized. Call Initialize() first.");

    public static MctsConfig Config => _config;

    /// <summary>
    /// Initialize MCTS resources. Call once before creating any MCTS players.
    /// </summary>
    /// <param name="modelPath">Path to the battle_model.onnx file.</param>
    /// <param name="vocabPath">Path to the battle_model_vocab.json file.</param>
    /// <param name="library">Game data library for species/move lookups.</param>
    /// <param name="config">Optional MCTS configuration. Uses defaults if null.</param>
    public static void Initialize(string modelPath, string vocabPath, Library library, MctsConfig? config = null)
    {
        _vocab = Vocab.Load(vocabPath, library);
        _encoder = new StateEncoder(_vocab);
        _model = new ModelInference(modelPath, _encoder);
        _config = config ?? new MctsConfig();
        _initialized = true;
    }

    /// <summary>
    /// Dispose of MCTS resources. Call when shutting down.
    /// </summary>
    public static void Shutdown()
    {
        _model?.Dispose();
        _model = null;
        _vocab = null;
        _encoder = null;
        _initialized = false;
    }
}
