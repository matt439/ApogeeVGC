namespace ApogeeVGC.Sim.Utils;

public class Rng(int seed) : Random(seed);

public record PrngSeed(int Seed);

public class Prng
{
    public PrngSeed StartingSeed { get; }
    private Rng Rng { get; }

    public Prng(PrngSeed? seed)
    {
        StartingSeed = seed ?? new PrngSeed(Environment.TickCount);
        Rng = new Rng(StartingSeed.Seed);
    }

    public PrngSeed GetSeed()
    {
        return new PrngSeed(StartingSeed.Seed);
    }

    /// <summary>
    /// Retrieves the next random number in the sequence.
    /// This function has three different results, depending on arguments:
    /// - Random() returns a real number in [0, 1), just like Math.random()
    /// - Random(n) returns an integer in [0, n)
    /// - Random(m, n) returns an integer in [m, n)
    /// </summary>
    public double Random()
    {
        return Rng.NextDouble();
    }

    /// <summary>
    /// Returns an integer in [0, max)
    /// </summary>
    public int Random(int max)
    {
        return Rng.Next(max);
    }

    /// <summary>
    /// Returns an integer in [min, max)
    /// </summary>
    public int Random(int min, int max)
    {
        return Rng.Next(min, max);
    }

    /// <summary>
    /// Flip a coin (two-sided die), returning true or false.
    /// This function returns true with probability P, where P = numerator / denominator.
    /// This function returns false with probability 1 - P.
    /// The numerator must be a non-negative integer (>= 0).
    /// The denominator must be a positive integer (> 0).
    /// </summary>
    public bool RandomChance(int numerator, int denominator)
    {
        if (denominator <= 0)
            throw new ArgumentException("Denominator must be positive", nameof(denominator));
        if (numerator < 0)
            throw new ArgumentException("Numerator must be non-negative", nameof(numerator));
        
        return Random(denominator) < numerator;
    }

    /// <summary>
    /// Return a random item from the given array.
    /// This function chooses items in the array with equal probability.
    /// If there are duplicate items in the array, each duplicate is
    /// considered separately. For example, Sample(['x', 'x', 'y']) returns
    /// 'x' 67% of the time and 'y' 33% of the time.
    /// The array must contain at least one item.
    /// </summary>
    public T Sample<T>(IReadOnlyList<T> items)
    {
        if (items.Count == 0)
            throw new ArgumentException("Cannot sample an empty array", nameof(items));
        
        int index = Random(items.Count);
        return items[index];
    }

    /// <summary>
    /// A Fisher-Yates shuffle. This is how the game resolves speed ties.
    /// At least according to V4 in
    /// https://github.com/smogon/pokemon-showdown/issues/1157#issuecomment-214454873
    /// </summary>
    public void Shuffle<T>(IList<T> items, int start = 0, int? end = null)
    {
        int endIndex = end ?? items.Count;
        
        while (start < endIndex - 1)
        {
            int nextIndex = Random(start, endIndex);
            if (start != nextIndex)
            {
                (items[start], items[nextIndex]) = (items[nextIndex], items[start]);
            }
            start++;
        }
    }
}