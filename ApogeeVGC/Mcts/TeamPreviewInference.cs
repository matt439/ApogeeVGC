using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Mcts;

/// <summary>
/// Result of a team preview model evaluation.
/// </summary>
public readonly struct TeamPreviewOutput
{
    /// <summary>Sigmoid scores [6] indicating which of my Pokemon to bring.</summary>
    public float[] BringScores { get; init; }

    /// <summary>Sigmoid scores [6] indicating which of my Pokemon to lead.</summary>
    public float[] LeadScores { get; init; }
}

/// <summary>
/// Wraps the ONNX InferenceSession for the team preview model.
/// Encodes a BattlePerspective at team preview into the model's input tensors
/// and produces bring/lead scores.
/// Thread-safe for concurrent reads (ONNX Runtime sessions are thread-safe).
/// </summary>
public sealed class TeamPreviewInference : IDisposable
{
    private const int NumSlots = 12; // 6 my Pokemon + 6 opponent Pokemon
    private const int NumMovesPerSlot = 4;

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
    /// The perspective must have PerspectiveType == TeamPreview.
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
        var resultList = results.ToList();

        float[] bringScores = resultList[0].AsTensor<float>().ToArray();
        float[] leadScores = resultList[1].AsTensor<float>().ToArray();

        return new TeamPreviewOutput { BringScores = bringScores, LeadScores = leadScores };
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

    public void Dispose()
    {
        _session.Dispose();
    }
}