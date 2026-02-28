using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using ApogeeVGC.Sim.BattleClasses;

namespace ApogeeVGC.Mcts;

/// <summary>
/// Result of a single model evaluation.
/// </summary>
public readonly struct ModelOutput
{
    public float Value { get; init; }
    public float[] PolicyA { get; init; }
    public float[] PolicyB { get; init; }
}

/// <summary>
/// Wraps the ONNX InferenceSession for the battle model.
/// Thread-safe for concurrent reads (ONNX Runtime sessions are thread-safe).
/// </summary>
public sealed class ModelInference : IDisposable
{
    private readonly InferenceSession _session;
    private readonly StateEncoder _encoder;

    public ModelInference(string onnxModelPath, StateEncoder encoder)
    {
        var sessionOptions = new SessionOptions();
        sessionOptions.GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL;
        _session = new InferenceSession(onnxModelPath, sessionOptions);
        _encoder = encoder;
    }

    /// <summary>
    /// Run inference on a single battle state.
    /// </summary>
    public ModelOutput Evaluate(BattlePerspective perspective)
    {
        var (speciesIds, numeric) = _encoder.Encode(perspective);

        var speciesTensor = new DenseTensor<long>(speciesIds, [1, StateEncoder.NumSpeciesSlots]);
        var numericTensor = new DenseTensor<float>(numeric, [1, StateEncoder.NumericDim]);

        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("species_ids", speciesTensor),
            NamedOnnxValue.CreateFromTensor("numeric", numericTensor),
        };

        using var results = _session.Run(inputs);
        var resultList = results.ToList();

        float value = resultList[0].AsTensor<float>()[0];
        float[] policyA = resultList[1].AsTensor<float>().ToArray();
        float[] policyB = resultList[2].AsTensor<float>().ToArray();

        return new ModelOutput { Value = value, PolicyA = policyA, PolicyB = policyB };
    }

    /// <summary>
    /// Apply softmax to raw logits, masked to only legal actions.
    /// Illegal actions get probability 0.
    /// </summary>
    public static float[] MaskedSoftmax(float[] logits, bool[] legalMask)
    {
        var result = new float[logits.Length];

        // Find max among legal actions for numerical stability
        float max = float.NegativeInfinity;
        for (int i = 0; i < logits.Length; i++)
        {
            if (legalMask[i] && logits[i] > max)
                max = logits[i];
        }

        if (float.IsNegativeInfinity(max))
        {
            // No legal actions — return uniform zeros
            return result;
        }

        // Compute exp and sum
        float sum = 0f;
        for (int i = 0; i < logits.Length; i++)
        {
            if (legalMask[i])
            {
                result[i] = MathF.Exp(logits[i] - max);
                sum += result[i];
            }
        }

        // Normalize
        if (sum > 0f)
        {
            for (int i = 0; i < result.Length; i++)
            {
                result[i] /= sum;
            }
        }

        return result;
    }

    public void Dispose()
    {
        _session.Dispose();
    }
}
