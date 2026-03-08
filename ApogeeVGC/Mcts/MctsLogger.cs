using System.Collections.Concurrent;

namespace ApogeeVGC.Mcts;

/// <summary>
/// Thread-safe diagnostic logger for MCTS and DL inference.
/// Collects value predictions and other metrics during evaluation runs.
/// Call <see cref="Reset"/> before an evaluation run and <see cref="GetSummary"/> after.
/// </summary>
public static class MctsLogger
{
    private static readonly ConcurrentBag<float> _valuePredictions = [];

    private static volatile bool _enabled;

    /// <summary>
    /// Enable or disable logging. Disabled by default to avoid overhead during normal play.
    /// </summary>
    public static bool Enabled
    {
        get => _enabled;
        set => _enabled = value;
    }

    /// <summary>
    /// Record a value head prediction.
    /// </summary>
    public static void LogValue(float value)
    {
        if (!_enabled) return;
        _valuePredictions.Add(value);
    }

    /// <summary>
    /// Clear all collected data. Call before starting an evaluation run.
    /// </summary>
    public static void Reset()
    {
        _valuePredictions.Clear();
    }

    /// <summary>
    /// Get a summary of collected value predictions.
    /// Returns null if no data was collected.
    /// </summary>
    public static ValueSummary? GetValueSummary()
    {
        float[] values = [.. _valuePredictions];
        if (values.Length == 0) return null;

        Array.Sort(values);

        float sum = 0f;
        foreach (float v in values) sum += v;
        float mean = sum / values.Length;

        float varianceSum = 0f;
        foreach (float v in values)
        {
            float diff = v - mean;
            varianceSum += diff * diff;
        }
        float stdDev = MathF.Sqrt(varianceSum / values.Length);

        int belowHalf = 0;
        int aroundHalf = 0;
        int aboveHalf = 0;
        foreach (float v in values)
        {
            if (v < 0.4f) belowHalf++;
            else if (v > 0.6f) aboveHalf++;
            else aroundHalf++;
        }

        return new ValueSummary
        {
            Count = values.Length,
            Mean = mean,
            StdDev = stdDev,
            Min = values[0],
            Max = values[^1],
            Median = values.Length % 2 == 0
                ? (values[values.Length / 2 - 1] + values[values.Length / 2]) / 2f
                : values[values.Length / 2],
            P10 = values[(int)(values.Length * 0.1f)],
            P90 = values[(int)(values.Length * 0.9f)],
            BelowHalf = belowHalf,
            AroundHalf = aroundHalf,
            AboveHalf = aboveHalf,
        };
    }

    /// <summary>
    /// Format the value summary as a multi-line string for console output.
    /// </summary>
    public static string FormatSummary()
    {
        ValueSummary? summary = GetValueSummary();
        if (summary == null) return "No value predictions logged.";

        var s = summary.Value;
        return $"""
            Value Head Diagnostics ({s.Count:N0} predictions):
              Mean: {s.Mean:F4}, StdDev: {s.StdDev:F4}
              Min: {s.Min:F4}, P10: {s.P10:F4}, Median: {s.Median:F4}, P90: {s.P90:F4}, Max: {s.Max:F4}
              Distribution: <0.4: {s.BelowHalf} ({(float)s.BelowHalf / s.Count:P1}), 0.4-0.6: {s.AroundHalf} ({(float)s.AroundHalf / s.Count:P1}), >0.6: {s.AboveHalf} ({(float)s.AboveHalf / s.Count:P1})
            """;
    }
}

public readonly struct ValueSummary
{
    public int Count { get; init; }
    public float Mean { get; init; }
    public float StdDev { get; init; }
    public float Min { get; init; }
    public float Max { get; init; }
    public float Median { get; init; }
    public float P10 { get; init; }
    public float P90 { get; init; }
    public int BelowHalf { get; init; }
    public int AroundHalf { get; init; }
    public int AboveHalf { get; init; }
}
