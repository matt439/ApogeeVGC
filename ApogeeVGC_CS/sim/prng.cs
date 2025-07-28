using System;

namespace ApogeeVGC_CS.sim
{
    // Seed types
    public class PrngSeed
    {
        public string Value { get; set; } = string.Empty;

        public PrngSeed(string value)
        {
            Value = value;
        }

        public static implicit operator string(PrngSeed seed) => seed.Value;
        public static implicit operator PrngSeed(string value) => new(value);

        public override string ToString() => Value;
    }

    public class SodiumRngSeed
    {
        public string Type { get; } = "sodium";
        public string Value { get; set; } = string.Empty;

        public SodiumRngSeed(string value)
        {
            Value = value;
        }
    }

    // 64-bit big-endian [high -> low] int
    public class Gen5RngSeed
    {
        public uint[] Values { get; set; } = new uint[4];

        public Gen5RngSeed(uint val1, uint val2, uint val3, uint val4)
        {
            Values = new[] { val1, val2, val3, val4 };
        }
    }

    // Low-level source of 32-bit random numbers
    public interface IRng
    {
        PrngSeed GetSeed();
        uint Next(); // random 32-bit number
    }

    // High-level PRNG API, for getting random numbers
    public class Prng
    {
        public PrngSeed StartingSeed { get; }
        public IRng Rng { get; private set; } = null!;

        /// <summary>
        /// Creates a new source of randomness for the given seed.
        /// Chooses the RNG implementation based on the seed passed to the constructor.
        /// Seeds starting with 'sodium' use sodium. Other seeds use the Gen 5 RNG.
        /// If a seed isn't given, defaults to sodium.
        /// </summary>
        public Prng(PrngSeed? seed = null, PrngSeed? initialSeed = null)
        {
            if (seed == null) seed = GenerateSeed();

            // Handle array compatibility for old input logs
            if (seed.Value.StartsWith('[') && seed.Value.EndsWith(']'))
            {
                // Parse array format and convert to comma-separated string
                seed = seed.Value.Trim('[', ']').Replace("\"", "");
            }

            if (string.IsNullOrEmpty(seed.Value))
            {
                throw new ArgumentException($"PRNG: Seed {seed} must be a string");
            }

            StartingSeed = initialSeed ?? seed;
            SetSeed(seed);
        }

        private static PrngSeed GenerateSeed()
        {
            // Generate a sodium seed by default
            var random = new Random();
            var randomString = Convert.ToBase64String(Guid.NewGuid().ToByteArray())[..8];
            return $"sodium,{randomString}";
        }

        private void SetSeed(PrngSeed seed)
        {
            var parts = seed.Value.Split(',', 2);
            if (parts.Length < 2)
            {
                throw new ArgumentException($"Invalid seed format: {seed}");
            }

            var seedType = parts[0];
            var seedValue = parts[1];

            switch (seedType)
            {
                case "sodium":
                    Rng = new SodiumRng(seedValue);
                    break;
                case "gen5":
                    Rng = new Gen5Rng(seedValue);
                    break;
                default:
                    // If it's a number, treat as Gen 5
                    if (int.TryParse(seedType, out _))
                    {
                        Rng = new Gen5Rng(seed.Value);
                    }
                    else
                    {
                        throw new ArgumentException($"Unknown seed type: {seedType}");
                    }
                    break;
            }
        }
    }

    // Placeholder implementations - these would need actual RNG logic
    public class SodiumRng : IRng
    {
        private readonly string _seed;

        public SodiumRng(string seed)
        {
            _seed = seed;
        }

        public PrngSeed GetSeed() => $"sodium,{_seed}";
        public uint Next() => 0; // TODO: Implement sodium RNG
    }

    public class Gen5Rng : IRng
    {
        private readonly string _seed;

        public Gen5Rng(string seed)
        {
            _seed = seed;
        }

        public PrngSeed GetSeed() => $"gen5,{_seed}";
        public uint Next() => 0; // TODO: Implement Gen 5 RNG
    }
}