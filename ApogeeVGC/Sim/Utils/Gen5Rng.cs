namespace ApogeeVGC.Sim.Utils;

/// <summary>
/// Port of Showdown's Gen5RNG — a Linear Congruential Generator (LCG) operating on a 64-bit state
/// represented as four 16-bit values [high → low].
///
/// x_{n+1} = (0x5D588B656C078965 * x_n + 0x00269EC3) mod 2^64
///
/// This produces identical random sequences to Showdown's TypeScript implementation,
/// enabling deterministic equivalence testing between the C# sim and Showdown.
/// </summary>
public sealed class Gen5Rng
{
    // LCG constants (same as Showdown)
    private static readonly ushort[] A = [0x5D58, 0x8B65, 0x6C07, 0x8965];
    private static readonly ushort[] C = [0x0000, 0x0000, 0x0026, 0x9EC3];

    private ushort[] _seed;

    public Gen5Rng(Gen5RngSeed seed)
    {
        _seed = [seed.S0, seed.S1, seed.S2, seed.S3];
    }

    public Gen5RngSeed GetSeed() => new(_seed[0], _seed[1], _seed[2], _seed[3]);

    /// <summary>
    /// Advance the RNG and return a 32-bit random number (upper 32 bits of state).
    /// Matches Showdown's Gen5RNG.next() exactly.
    /// </summary>
    public uint Next()
    {
        _seed = NextFrame(_seed);
        // Use the upper 32 bits: (seed[0] << 16) + seed[1]
        return ((uint)_seed[0] << 16) + _seed[1];
    }

    /// <summary>
    /// Calculates a * b + c using 64-bit arithmetic represented as four 16-bit values.
    /// Exact port of Showdown's multiplyAdd.
    /// </summary>
    private static ushort[] MultiplyAdd(ushort[] a, ushort[] b, ushort[] c)
    {
        var result = new ushort[4];
        uint carry = 0;

        for (int outIndex = 3; outIndex >= 0; outIndex--)
        {
            for (int bIndex = outIndex; bIndex < 4; bIndex++)
            {
                int aIndex = 3 - (bIndex - outIndex);
                carry += (uint)a[aIndex] * b[bIndex];
            }
            carry += c[outIndex];

            result[outIndex] = (ushort)(carry & 0xFFFF);
            carry >>= 16;
        }

        return result;
    }

    private static ushort[] NextFrame(ushort[] seed)
    {
        return MultiplyAdd(seed, A, C);
    }
}

/// <summary>
/// Seed for Gen5RNG: four 16-bit values representing a 64-bit state [high → low].
/// </summary>
public readonly record struct Gen5RngSeed(ushort S0, ushort S1, ushort S2, ushort S3)
{
    /// <summary>
    /// Parse from Showdown's comma-separated format: "n0,n1,n2,n3"
    /// </summary>
    public static Gen5RngSeed Parse(string s)
    {
        string[] parts = s.Split(',');
        if (parts.Length != 4)
            throw new ArgumentException($"Expected 4 comma-separated values, got {parts.Length}");
        return new Gen5RngSeed(
            ushort.Parse(parts[0]),
            ushort.Parse(parts[1]),
            ushort.Parse(parts[2]),
            ushort.Parse(parts[3]));
    }

    public override string ToString() => $"{S0},{S1},{S2},{S3}";
}
