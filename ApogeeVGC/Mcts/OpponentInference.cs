using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using ApogeeVGC.Sim.BattleClasses;

namespace ApogeeVGC.Mcts;

/// <summary>
/// Result of opponent prediction model inference.
/// Contains raw logits for opponent action predictions per active slot.
/// </summary>
public readonly struct OpponentPrediction
{
    /// <summary>Raw logits for opponent slot A actions.</summary>
    public float[] PolicyA { get; init; }
    /// <summary>Raw logits for opponent slot B actions.</summary>
    public float[] PolicyB { get; init; }
}

/// <summary>
/// Wraps the ONNX InferenceSession for the opponent prediction model.
/// Thread-safe for concurrent reads (ONNX Runtime sessions are thread-safe).
/// </summary>
public sealed class OpponentInference : IDisposable
{
    private readonly InferenceSession _session;
    private readonly StateEncoder _encoder;

    public OpponentInference(string onnxModelPath, StateEncoder encoder)
    {
        var sessionOptions = new SessionOptions();
        sessionOptions.GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL;
        _session = new InferenceSession(Path.GetFullPath(onnxModelPath), sessionOptions);
        _encoder = encoder;
    }

    /// <summary>
    /// Predict what the opponent will do given the current battle state.
    /// </summary>
    public OpponentPrediction Predict(BattlePerspective perspective)
    {
        EncodedState state = _encoder.Encode(perspective);

        var speciesTensor = new DenseTensor<long>(state.SpeciesIds, [1, StateEncoder.NumSpeciesSlots]);
        var moveTensor = new DenseTensor<long>(state.MoveIds, [1, StateEncoder.NumSpeciesSlots, StateEncoder.NumMoveSlotsPerPokemon]);
        var abilityTensor = new DenseTensor<long>(state.AbilityIds, [1, StateEncoder.NumSpeciesSlots]);
        var itemTensor = new DenseTensor<long>(state.ItemIds, [1, StateEncoder.NumSpeciesSlots]);
        var teraTensor = new DenseTensor<long>(state.TeraIds, [1, StateEncoder.NumSpeciesSlots]);
        var numericTensor = new DenseTensor<float>(state.Numeric, [1, StateEncoder.NumericDim]);

        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("species_ids", speciesTensor),
            NamedOnnxValue.CreateFromTensor("move_ids", moveTensor),
            NamedOnnxValue.CreateFromTensor("ability_ids", abilityTensor),
            NamedOnnxValue.CreateFromTensor("item_ids", itemTensor),
            NamedOnnxValue.CreateFromTensor("tera_ids", teraTensor),
            NamedOnnxValue.CreateFromTensor("numeric", numericTensor),
        };

        using var results = _session.Run(inputs);
        var resultList = results.ToList();

        float[] policyA = resultList[0].AsTensor<float>().ToArray();
        float[] policyB = resultList[1].AsTensor<float>().ToArray();

        return new OpponentPrediction { PolicyA = policyA, PolicyB = policyB };
    }

    public void Dispose()
    {
        _session.Dispose();
    }
}
