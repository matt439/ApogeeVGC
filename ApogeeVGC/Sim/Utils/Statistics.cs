namespace ApogeeVGC.Sim.Utils;

public static class Statistics
{
    public static double Mean(this IEnumerable<int> source)
    {
        if (source == null)
            throw new ArgumentException("Source cannot be null.");

        var list = source.ToList();
        if (list.Count == 0)
            throw new ArgumentException("Source cannot be empty.");

        return list.Average();
    }

    public static double StandardDeviation(this IEnumerable<int> source)
    {
        if (source == null)
            throw new ArgumentException("Source cannot be null.");

        var list = source.ToList();
        if (list.Count == 0)
            throw new ArgumentException("Source cannot be empty.");

        double mean = list.Average();
        double variance = list.Select(x => Math.Pow(x - mean, 2)).Average();
        return Math.Sqrt(variance);
    }

    public static double Median(this IEnumerable<int> source)
    {
        if (source == null)
            throw new ArgumentException("Source cannot be null.");

        var list = source.ToList();
        if (list.Count == 0)
            throw new ArgumentException("Source cannot be empty.");

        var sorted = list.OrderBy(x => x).ToList();
        int count = sorted.Count;

        if (count % 2 == 0)
        {
            // Even number of elements - average of middle two
            return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
        }
        else
        {
            // Odd number of elements - middle element
            return sorted[count / 2];
        }
    }

    public static int Minimum(this IEnumerable<int> source)
    {
        if (source == null)
            throw new ArgumentException("Source cannot be null.");

        var list = source.ToList();
        if (list.Count == 0)
            throw new ArgumentException("Source cannot be empty.");

        return list.Min();
    }

    public static int Maximum(this IEnumerable<int> source)
    {
        if (source == null)
            throw new ArgumentException("Source cannot be null.");

        var list = source.ToList();
        if (list.Count == 0)
            throw new ArgumentException("Source cannot be empty.");

        return list.Max();
    }
}