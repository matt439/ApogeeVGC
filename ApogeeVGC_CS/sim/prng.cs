namespace ApogeeVGC_CS.sim
{
    // Seed types
    public class PrngSeed(string value)
    {
        public string Value { get; set; } = value;

        public static implicit operator string(PrngSeed seed) => seed.Value;
        public static implicit operator PrngSeed(string value) => new(value);

        public override string ToString() => Value;
    }

    public class SodiumRngSeed(string value)
    {
        public string Type { get; } = "sodium";
        public string Value { get; set; } = value;
    }

    // 64-bit big-endian [high -> low] int
    public class Gen5RngSeed(uint val1, uint val2, uint val3, uint val4)
    {
        public uint[] Values { get; set; } = [val1, val2, val3, val4];
    }

    // Low-level source of 32-bit random numbers
    public interface IRng
    {
        public PrngSeed GetSeed();
        public uint Next(); // random 32-bit number
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
            seed ??= GenerateSeed();

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
            //var random = new Random();
            string randomString = Convert.ToBase64String(Guid.NewGuid().ToByteArray())[..8];
            return $"sodium,{randomString}";
        }

        private void SetSeed(PrngSeed seed)
        {
            string[] parts = seed.Value.Split(',', 2);
            if (parts.Length < 2)
            {
                throw new ArgumentException($"Invalid seed format: {seed}");
            }

            string seedType = parts[0];
            string seedValue = parts[1];

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
    public class SodiumRng(string seed) : IRng
    {
        public PrngSeed GetSeed() => $"sodium,{seed}";
        public uint Next() => 0; // TODO: Implement sodium RNG
    }

    public class Gen5Rng(string seed) : IRng
    {
        public PrngSeed GetSeed() => $"gen5,{seed}";
        public uint Next() => 0; // TODO: Implement Gen 5 RNG
    }
}