namespace ApogeeVGC.Sim.Utils;

public class Rng(int seed) : Random(seed);

public record PrngSeed(int Seed)
{
    /// <summary>
    /// Create a PrngSeed from a Gen5RNG seed, for use in equivalence testing.
    /// The Gen5RngSeed is stored internally; the int Seed is unused in Gen5 mode.
    /// </summary>
    public Gen5RngSeed? Gen5Seed { get; init; }

    public static PrngSeed FromGen5(Gen5RngSeed gen5Seed) =>
        new(0) { Gen5Seed = gen5Seed };
}

public class Prng
{
    public PrngSeed StartingSeed { get; }

    // One of these is non-null depending on mode
    private Rng? _dotnetRng;
    private Gen5Rng? _gen5Rng;

    /// <summary>Access the underlying Gen5Rng (null if in .NET mode).</summary>
    public Gen5Rng? Gen5 => _gen5Rng;

    public Prng(PrngSeed? seed)
    {
        if (seed?.Gen5Seed is { } gen5Seed)
        {
            // Gen5RNG mode — matches Showdown's PRNG exactly
            StartingSeed = seed;
            _gen5Rng = new Gen5Rng(gen5Seed);
        }
        else
        {
            // Default .NET Random mode — fast, for training/MCTS
            StartingSeed = seed ?? new PrngSeed(Environment.TickCount);
            _dotnetRng = new Rng(StartingSeed.Seed);
        }
    }

    public PrngSeed GetSeed()
    {
        return StartingSeed;
    }

    /// <summary>
    /// Returns a real number in [0, 1)
    /// </summary>
    public double Random()
    {
        if (_gen5Rng != null)
            return _gen5Rng.Next() / (double)(1L << 32);
        return _dotnetRng!.NextDouble();
    }

    /// <summary>
    /// Returns an integer in [0, max)
    /// </summary>
    public int Random(int max)
    {
        if (_gen5Rng != null)
        {
            // Match Showdown: Math.floor(result * from / 2**32)
            uint result = _gen5Rng.Next();
            int val = (int)(result * (ulong)max >> 32);
            return val;
        }
        return _dotnetRng!.Next(max);
    }

    /// <summary>
    /// Returns an integer in [min, max)
    /// </summary>
    public int Random(int min, int max)
    {
        if (_gen5Rng != null)
        {
            // Match Showdown: Math.floor(result * (to - from) / 2**32) + from
            uint result = _gen5Rng.Next();
            int val = (int)(result * (ulong)(max - min) >> 32) + min;
            return val;
        }
        return _dotnetRng!.Next(min, max);
    }

    /// <summary>
    /// Flip a coin (two-sided die), returning true or false.
    /// This function returns true with probability P, where P = numerator / denominator.
    /// This function returns false with probability 1 - P.
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
