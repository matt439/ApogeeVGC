using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Mcts;

/// <summary>
/// A single VGC team preview configuration: which 4 to bring, which 2 to lead.
/// </summary>
public readonly struct VgcConfig
{
    public int[] Bring { get; init; }
    public int[] Lead { get; init; }
    public int[] Bench { get; init; }
}

/// <summary>
/// Result of a team preview model evaluation.
/// </summary>
public readonly struct TeamPreviewOutput
{
    /// <summary>Ordered team indices: leads first, then bench (4 total for VGC).</summary>
    public int[] OrderedIndices { get; init; }

    /// <summary>Softmax probability of the selected configuration.</summary>
    public float Confidence { get; init; }

    /// <summary>All 90 configuration softmax probabilities.</summary>
    public float[] ConfigScores { get; init; }

    /// <summary>Index of the selected configuration (0-89).</summary>
    public int ConfigIndex { get; init; }
}

/// <summary>
/// Wraps the ONNX InferenceSession for the VGC team preview model.
/// The model outputs 90 logits over all possible VGC configurations
/// (C(6,4) bring × C(4,2) lead = 90).
/// Thread-safe for concurrent reads (ONNX Runtime sessions are thread-safe).
/// </summary>
public sealed class TeamPreviewInference : IDisposable
{
    private const int NumSlots = 12; // 6 my Pokemon + 6 opponent Pokemon
    private const int NumMovesPerSlot = 4;
    private const int NumVgcConfigs = 90;

    /// <summary>All 90 VGC configurations in canonical order (matches Python).</summary>
    public static readonly VgcConfig[] VgcConfigs = GenerateVgcConfigs();

    private readonly InferenceSession _session;
    private readonly Vocab _vocab;

    public TeamPreviewInference(string onnxModelPath, Vocab vocab)
    {
        var sessionOptions = new SessionOptions();
        sessionOptions.GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL;
        _session = new InferenceSession(Path.GetFullPath(onnxModelPath), sessionOptions);
        _vocab = vocab;
    }

    /// <summary>
    /// Run inference on a team preview perspective.
    /// Returns the best configuration with ordered indices (leads first, then bench).
    /// </summary>
    public TeamPreviewOutput Evaluate(BattlePerspective perspective)
    {
        var speciesIds = new long[NumSlots];
        var moveIds = new long[NumSlots * NumMovesPerSlot];
        var abilityIds = new long[NumSlots];
        var itemIds = new long[NumSlots];
        var teraIds = new long[NumSlots];

        // Encode my 6 Pokemon (slots 0..5)
        var myPokemon = perspective.PlayerSide.Pokemon;
        for (var i = 0; i < myPokemon.Count && i < 6; i++)
            EncodeSlot(speciesIds, moveIds, abilityIds, itemIds, teraIds, i, myPokemon[i]);

        // Encode opponent's 6 Pokemon (slots 6..11)
        var oppPokemon = perspective.OpponentSide.Pokemon;
        for (var i = 0; i < oppPokemon.Count && i < 6; i++)
            EncodeSlot(speciesIds, moveIds, abilityIds, itemIds, teraIds, 6 + i, oppPokemon[i]);

        // Create tensors
        var speciesTensor = new DenseTensor<long>(speciesIds, [1, NumSlots]);
        var moveTensor = new DenseTensor<long>(moveIds, [1, NumSlots, NumMovesPerSlot]);
        var abilityTensor = new DenseTensor<long>(abilityIds, [1, NumSlots]);
        var itemTensor = new DenseTensor<long>(itemIds, [1, NumSlots]);
        var teraTensor = new DenseTensor<long>(teraIds, [1, NumSlots]);

        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("species_ids", speciesTensor),
            NamedOnnxValue.CreateFromTensor("move_ids", moveTensor),
            NamedOnnxValue.CreateFromTensor("ability_ids", abilityTensor),
            NamedOnnxValue.CreateFromTensor("item_ids", itemTensor),
            NamedOnnxValue.CreateFromTensor("tera_ids", teraTensor),
        };

        using var results = _session.Run(inputs);
        float[] logits = results.First().AsTensor<float>().ToArray();

        // Softmax
        float[] probs = Softmax(logits);

        // Argmax
        int bestIdx = 0;
        for (int i = 1; i < probs.Length; i++)
        {
            if (probs[i] > probs[bestIdx])
                bestIdx = i;
        }

        VgcConfig config = VgcConfigs[bestIdx];

        // Ordered: leads first, then bench
        int[] ordered = new int[config.Lead.Length + config.Bench.Length];
        config.Lead.CopyTo(ordered, 0);
        config.Bench.CopyTo(ordered, config.Lead.Length);

        return new TeamPreviewOutput
        {
            OrderedIndices = ordered,
            Confidence = probs[bestIdx],
            ConfigScores = probs,
            ConfigIndex = bestIdx,
        };
    }

    private void EncodeSlot(
        long[] speciesIds, long[] moveIds, long[] abilityIds,
        long[] itemIds, long[] teraIds, int slot, PokemonPerspective p)
    {
        speciesIds[slot] = _vocab.GetSpeciesIndex(p.Species);

        var moves = p.MoveSlots;
        int moveBase = slot * NumMovesPerSlot;
        for (var j = 0; j < moves.Count && j < NumMovesPerSlot; j++)
            moveIds[moveBase + j] = _vocab.GetMoveEmbedIndex(moves[j].Move);

        abilityIds[slot] = _vocab.GetAbilityIndex(p.Ability);
        itemIds[slot] = _vocab.GetItemIndex(p.Item);
        teraIds[slot] = _vocab.GetTeraTypeIndex(p.TeraType);
    }

    private static float[] Softmax(float[] logits)
    {
        float max = logits[0];
        for (int i = 1; i < logits.Length; i++)
            if (logits[i] > max) max = logits[i];

        float[] exp = new float[logits.Length];
        float sum = 0;
        for (int i = 0; i < logits.Length; i++)
        {
            exp[i] = MathF.Exp(logits[i] - max);
            sum += exp[i];
        }
        for (int i = 0; i < exp.Length; i++)
            exp[i] /= sum;

        return exp;
    }

    /// <summary>
    /// Generate all 90 VGC configurations in lexicographic order.
    /// Matches the Python itertools.combinations enumeration exactly.
    /// </summary>
    private static VgcConfig[] GenerateVgcConfigs()
    {
        var configs = new List<VgcConfig>();

        // C(6,4) bring combinations in lexicographic order
        for (int a = 0; a < 3; a++)
        for (int b = a + 1; b < 4; b++)
        for (int c = b + 1; c < 5; c++)
        for (int d = c + 1; d < 6; d++)
        {
            int[] bring = [a, b, c, d];

            // C(4,2) lead combinations from bring indices
            for (int li = 0; li < 3; li++)
            for (int lj = li + 1; lj < 4; lj++)
            {
                int[] lead = [bring[li], bring[lj]];
                var bench = new List<int>();
                for (int k = 0; k < 4; k++)
                    if (k != li && k != lj)
                        bench.Add(bring[k]);

                configs.Add(new VgcConfig
                {
                    Bring = bring,
                    Lead = lead,
                    Bench = bench.ToArray(),
                });
            }
        }

        return configs.ToArray();
    }

    public void Dispose()
    {
        _session.Dispose();
    }
}
