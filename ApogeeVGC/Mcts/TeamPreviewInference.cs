using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Mcts;

/// <summary>
/// A team preview configuration: which pokemon to bring, lead, and bench.
/// </summary>
public readonly struct PreviewConfig
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
    /// <summary>Ordered team indices: leads first, then bench.</summary>
    public int[] OrderedIndices { get; init; }

    /// <summary>Softmax probability of the selected configuration.</summary>
    public float Confidence { get; init; }

    /// <summary>All configuration softmax probabilities.</summary>
    public float[] ConfigScores { get; init; }

    /// <summary>Index of the selected configuration.</summary>
    public int ConfigIndex { get; init; }
}

/// <summary>
/// Wraps the ONNX InferenceSession for the team preview model.
/// Supports any format (VGC, Doubles OU, Singles) — the config enumeration
/// is generated from teamSize and numLeads parameters.
/// Thread-safe for concurrent reads (ONNX Runtime sessions are thread-safe).
/// </summary>
public sealed class TeamPreviewInference : IDisposable
{
    private const int NumSlots = 12; // 6 my Pokemon + 6 opponent Pokemon
    private const int NumMovesPerSlot = 4;

    /// <summary>All configurations in canonical order (matches Python).</summary>
    public PreviewConfig[] Configs { get; }

    private readonly InferenceSession _session;
    private readonly Vocab _vocab;

    public TeamPreviewInference(string onnxModelPath, Vocab vocab,
        int teamSize = 4, int numLeads = 2, int totalPokemon = 6)
    {
        var sessionOptions = new SessionOptions();
        sessionOptions.GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL;
        _session = new InferenceSession(Path.GetFullPath(onnxModelPath), sessionOptions);
        _vocab = vocab;
        Configs = GenerateConfigs(totalPokemon, teamSize, numLeads);
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

        PreviewConfig config = Configs[bestIdx];

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
    /// Generate all configurations in lexicographic order for the given format.
    /// Matches the Python itertools.combinations enumeration exactly.
    /// </summary>
    public static PreviewConfig[] GenerateConfigs(int totalPokemon, int teamSize, int numLeads)
    {
        var configs = new List<PreviewConfig>();

        foreach (int[] bring in Combinations(totalPokemon, teamSize))
        {
            foreach (int[] lead in CombinationsFrom(bring, numLeads))
            {
                var leadSet = new HashSet<int>(lead);
                var bench = new List<int>();
                foreach (int b in bring)
                    if (!leadSet.Contains(b))
                        bench.Add(b);

                configs.Add(new PreviewConfig
                {
                    Bring = bring,
                    Lead = lead,
                    Bench = bench.ToArray(),
                });
            }
        }

        return configs.ToArray();
    }

    /// <summary>Generate all C(n, k) combinations of {0..n-1} in lexicographic order.</summary>
    private static IEnumerable<int[]> Combinations(int n, int k)
    {
        int[] indices = new int[k];
        for (int i = 0; i < k; i++) indices[i] = i;

        yield return (int[])indices.Clone();

        while (true)
        {
            int i = k - 1;
            while (i >= 0 && indices[i] == i + n - k) i--;
            if (i < 0) yield break;
            indices[i]++;
            for (int j = i + 1; j < k; j++) indices[j] = indices[j - 1] + 1;
            yield return (int[])indices.Clone();
        }
    }

    /// <summary>Generate all C(source.Length, k) combinations from a source array.</summary>
    private static IEnumerable<int[]> CombinationsFrom(int[] source, int k)
    {
        int n = source.Length;
        int[] indices = new int[k];
        for (int i = 0; i < k; i++) indices[i] = i;

        yield return indices.Select(i => source[i]).ToArray();

        while (true)
        {
            int i = k - 1;
            while (i >= 0 && indices[i] == i + n - k) i--;
            if (i < 0) yield break;
            indices[i]++;
            for (int j = i + 1; j < k; j++) indices[j] = indices[j - 1] + 1;
            yield return indices.Select(idx => source[idx]).ToArray();
        }
    }

    public void Dispose()
    {
        _session.Dispose();
    }
}
