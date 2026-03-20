using System.Text.Json;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Mcts.Ensemble;

/// <summary>
/// Configuration for ensemble weights, loadable from JSON.
/// </summary>
public sealed class EnsembleConfig
{
    /// <summary>
    /// Global weight per mini-model. Keys are mini-model names,
    /// values are non-negative weights. Missing models default to 1.0.
    /// </summary>
    public Dictionary<string, float> Weights { get; set; } = new();

    public static EnsembleConfig Load(string path)
    {
        if (!File.Exists(path))
            return new EnsembleConfig();

        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<EnsembleConfig>(json)
               ?? new EnsembleConfig();
    }

    public float GetWeight(string modelName)
    {
        return Weights.TryGetValue(modelName, out float w) ? w : 1.0f;
    }
}

/// <summary>
/// Aggregates scores from multiple mini-models into MCTS edge priors.
///
/// Formula: final(a) = Sum(weight_i * confidence_i * pref_i(a)) / Sum(weight_i * confidence_i)
///
/// The result is a normalized probability distribution over edges,
/// suitable for use as MCTS priors.
/// </summary>
public sealed class EnsembleEvaluator
{
    private readonly IMiniModel[] _models;
    private readonly EnsembleConfig _config;

    public EnsembleEvaluator(IMiniModel[] models, EnsembleConfig config)
    {
        _models = models;
        _config = config;
    }

    /// <summary>
    /// Score candidate edges and return normalized prior probabilities.
    /// Falls back to uniform priors if all models have zero confidence.
    /// </summary>
    public float[] ScoreEdges(
        Battle battle,
        SideId sideId,
        IReadOnlyList<MctsEdge> edges,
        OpponentPrediction? opponentPrediction,
        BattleInfoTracker? tracker = null)
    {
        int n = edges.Count;
        if (n == 0) return [];

        float[] combined = new float[n];
        float totalWeight = 0f;

        foreach (IMiniModel model in _models)
        {
            float globalWeight = _config.GetWeight(model.Name);
            if (globalWeight <= 0f) continue;

            MiniModelScore[] scores = model.Evaluate(
                battle, sideId, edges, opponentPrediction, tracker);

            for (int i = 0; i < n; i++)
            {
                float w = globalWeight * scores[i].Confidence;
                combined[i] += w * scores[i].Preference;
                totalWeight += w;
            }
        }

        // Normalize to probability distribution
        if (totalWeight > 0f)
        {
            float sum = 0f;
            for (int i = 0; i < n; i++)
            {
                combined[i] /= totalWeight;
                sum += combined[i];
            }

            // Normalize to sum to 1
            if (sum > 0f)
            {
                for (int i = 0; i < n; i++)
                    combined[i] /= sum;
            }
            else
            {
                // All preferences are zero — fall back to uniform
                float uniform = 1f / n;
                for (int i = 0; i < n; i++)
                    combined[i] = uniform;
            }
        }
        else
        {
            // No model has any confidence — uniform priors
            float uniform = 1f / n;
            for (int i = 0; i < n; i++)
                combined[i] = uniform;
        }

        return combined;
    }
}
