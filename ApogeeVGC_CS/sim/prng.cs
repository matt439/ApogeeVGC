using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ApogeeVGC_CS.sim
{
    /// <summary>
    /// PRNG seed format: "sodium,{hexstring}" or "gen5,{hexstring}" or "{number},{number},{number},{number}"
    /// </summary>
    public class PrngSeed
    {
        public string Value { get; }

        public PrngSeed(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static implicit operator PrngSeed(string value) => new(value);
        public static implicit operator string(PrngSeed seed) => seed.Value;

        public override string ToString() => Value;
    }

    /// <summary>
    /// Sodium RNG seed: ["sodium", hexstring]
    /// </summary>
    public record SodiumRngSeed(string Type, string HexString)
    {
        public SodiumRngSeed() : this("sodium", "") { }
    }

    /// <summary>
    /// 64-bit big-endian [high -> low] int represented as 4 16-bit values
    /// </summary>
    public record Gen5RngSeed(int Value0, int Value1, int Value2, int Value3)
    {
        public int[] ToArray() => [Value0, Value1, Value2, Value3];

        public static Gen5RngSeed FromArray(int[] values)
        {
            if (values.Length != 4)
                throw new ArgumentException("Gen5RngSeed requires exactly 4 values");
            return new Gen5RngSeed(values[0], values[1], values[2], values[3]);
        }
    }

    /// <summary>
    /// Low-level source of 32-bit random numbers.
    /// </summary>
    public interface IRng
    {
        PrngSeed GetSeed();
        /// <summary>Random 32-bit number</summary>
        uint Next();
    }

    /// <summary>
    /// ChaCha20 stream cipher implementation
    /// </summary>
    internal static class ChaCha20
    {
        private const uint ConstantByte0 = 0x61707865; // "expa"
        private const uint ConstantByte1 = 0x3320646e; // "nd 3"
        private const uint ConstantByte2 = 0x79622d32; // "2-by"
        private const uint ConstantByte3 = 0x6b206574; // "te k"

        public static byte[] Encrypt(byte[] key, byte[] nonce, byte[] data, uint counter = 0)
        {
            if (key.Length != 32)
                throw new ArgumentException("Key must be 32 bytes");
            if (nonce.Length != 12)
                throw new ArgumentException("Nonce must be 12 bytes");

            byte[] result = new byte[data.Length];
            uint[] state = new uint[16];

            // Initialize state
            state[0] = ConstantByte0;
            state[1] = ConstantByte1;
            state[2] = ConstantByte2;
            state[3] = ConstantByte3;

            // Key (32 bytes = 8 uint32s)
            for (int i = 0; i < 8; i++)
            {
                state[4 + i] = BitConverter.ToUInt32(key, i * 4);
            }

            // Counter (4 bytes = 1 uint32)
            state[12] = counter;

            // Nonce (12 bytes = 3 uint32s)
            for (int i = 0; i < 3; i++)
            {
                state[13 + i] = BitConverter.ToUInt32(nonce, i * 4);
            }

            // Process data in 64-byte blocks
            int blockCount = (data.Length + 63) / 64;
            for (int block = 0; block < blockCount; block++)
            {
                byte[] keyStream = GenerateKeyStreamBlock(state);

                int blockSize = Math.Min(64, data.Length - block * 64);
                for (int i = 0; i < blockSize; i++)
                {
                    result[block * 64 + i] = (byte)(data[block * 64 + i] ^ keyStream[i]);
                }

                // Increment counter
                state[12]++;
            }

            return result;
        }

        private static byte[] GenerateKeyStreamBlock(uint[] state)
        {
            uint[] workingState = new uint[16];
            Array.Copy(state, workingState, 16);

            // 20 rounds (10 double rounds)
            for (int i = 0; i < 10; i++)
            {
                // Column rounds
                QuarterRound(workingState, 0, 4, 8, 12);
                QuarterRound(workingState, 1, 5, 9, 13);
                QuarterRound(workingState, 2, 6, 10, 14);
                QuarterRound(workingState, 3, 7, 11, 15);

                // Diagonal rounds
                QuarterRound(workingState, 0, 5, 10, 15);
                QuarterRound(workingState, 1, 6, 11, 12);
                QuarterRound(workingState, 2, 7, 8, 13);
                QuarterRound(workingState, 3, 4, 9, 14);
            }

            // Add original state
            for (int i = 0; i < 16; i++)
            {
                workingState[i] += state[i];
            }

            // Convert to bytes
            byte[] result = new byte[64];
            for (int i = 0; i < 16; i++)
            {
                byte[] bytes = BitConverter.GetBytes(workingState[i]);
                Buffer.BlockCopy(bytes, 0, result, i * 4, 4);
            }

            return result;
        }

        private static void QuarterRound(uint[] state, int a, int b, int c, int d)
        {
            state[a] += state[b];
            state[d] ^= state[a];
            state[d] = RotateLeft(state[d], 16);

            state[c] += state[d];
            state[b] ^= state[c];
            state[b] = RotateLeft(state[b], 12);

            state[a] += state[b];
            state[d] ^= state[a];
            state[d] = RotateLeft(state[d], 8);

            state[c] += state[d];
            state[b] ^= state[c];
            state[b] = RotateLeft(state[b], 7);
        }

        private static uint RotateLeft(uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }
    }

    /// <summary>
    /// High-level PRNG API, for getting random numbers.
    /// 
    /// Chooses the RNG implementation based on the seed passed to the constructor.
    /// Seeds starting with 'sodium' use sodium. Other seeds use the Gen 5 RNG.
    /// If a seed isn't given, defaults to sodium.
    /// 
    /// The actual randomness source is in this.rng.
    /// </summary>
    public class Prng
    {
        public PrngSeed StartingSeed { get; }
        private IRng rng = new SodiumRng(SodiumRng.GenerateSeed());

        /// <summary>Creates a new source of randomness for the given seed.</summary>
        public Prng(PrngSeed? seed = null, PrngSeed? initialSeed = null)
        {
            seed ??= GenerateSeed();

            // Handle array compatibility for old input logs
            if (seed.Value.Contains(',') && !seed.Value.StartsWith("sodium,") && !seed.Value.StartsWith("gen5,"))
            {
                string[] parts = seed.Value.Split(',');
                if (parts.Length == 4 && parts.All(p => int.TryParse(p, out _)))
                {
                    seed = new PrngSeed(string.Join(",", parts));
                }
            }

            StartingSeed = initialSeed ?? seed;
            SetSeed(seed);
        }

        public void SetSeed(PrngSeed seed)
        {
            if (seed.Value.StartsWith("sodium,"))
            {
                string[] parts = seed.Value.Split(',');
                rng = new SodiumRng(new SodiumRngSeed(parts[0], parts[1]));
            }
            else if (seed.Value.StartsWith("gen5,"))
            {
                string hexSeed = seed.Value[5..];
                Gen5RngSeed gen5Seed = new Gen5RngSeed(
                    Convert.ToInt32(hexSeed[0..4], 16),
                    Convert.ToInt32(hexSeed[4..8], 16),
                    Convert.ToInt32(hexSeed[8..12], 16),
                    Convert.ToInt32(hexSeed[12..16], 16)
                );
                rng = new Gen5Rng(gen5Seed);
            }
            else if (Regex.IsMatch(seed.Value, @"^[0-9]"))
            {
                int[] parts = seed.Value.Split(',').Select(int.Parse).ToArray();
                rng = new Gen5Rng(Gen5RngSeed.FromArray(parts));
            }
            else
            {
                throw new ArgumentException($"Unrecognized RNG seed {seed}");
            }
        }

        public PrngSeed GetSeed() => rng.GetSeed();

        /// <summary>
        /// Creates a clone of the current PRNG.
        /// 
        /// The new PRNG will have its initial seed set to the seed of the current instance.
        /// </summary>
        public Prng Clone() => new(rng.GetSeed(), StartingSeed);

        /// <summary>
        /// Retrieves the next random number in the sequence.
        /// This function has three different results, depending on arguments:
        /// - Random() returns a real number in [0, 1), just like Math.Random()
        /// - Random(n) returns an integer in [0, n)
        /// - Random(m, n) returns an integer in [m, n)
        /// m and n are converted to integers via Math.Floor. If the result is NaN, they are ignored.
        /// </summary>
        public double Random(int? from = null, int? to = null)
        {
            uint result = rng.Next();

            if (from.HasValue) from = (int)Math.Floor((double)from.Value);
            if (to.HasValue) to = (int)Math.Floor((double)to.Value);

            if (!from.HasValue)
            {
                return result / Math.Pow(2, 32);
            }
            else if (!to.HasValue)
            {
                return Math.Floor(result * from.Value / Math.Pow(2, 32));
            }
            else
            {
                return Math.Floor(result * (to.Value - from.Value) / Math.Pow(2, 32)) + from.Value;
            }
        }

        /// <summary>
        /// Flip a coin (two-sided die), returning true or false.
        /// 
        /// This function returns true with probability P, where P = numerator / denominator.
        /// This function returns false with probability 1 - P.
        /// 
        /// The numerator must be a non-negative integer (>= 0).
        /// The denominator must be a positive integer (> 0).
        /// </summary>
        public bool RandomChance(int numerator, int denominator)
        {
            return Random(denominator) < numerator;
        }

        /// <summary>
        /// Return a random item from the given array.
        /// 
        /// This function chooses items in the array with equal probability.
        /// 
        /// If there are duplicate items in the array, each duplicate is
        /// considered separately. For example, Sample(['x', 'x', 'y']) returns
        /// 'x' 67% of the time and 'y' 33% of the time.
        /// 
        /// The array must contain at least one item.
        /// The array must not be sparse.
        /// </summary>
        public T Sample<T>(IReadOnlyList<T> items)
        {
            if (items.Count == 0)
            {
                throw new ArgumentException("Cannot sample an empty array");
            }

            int index = (int)Random(items.Count);
            T item = items[index];

            return item;
        }

        /// <summary>
        /// A Fisher-Yates shuffle. This is how the game resolves speed ties.
        /// 
        /// At least according to V4 in
        /// https://github.com/smogon/pokemon-showdown/issues/1157#issuecomment-214454873
        /// </summary>
        public void Shuffle<T>(IList<T> items, int start = 0, int? end = null)
        {
            end ??= items.Count;

            while (start < end - 1)
            {
                int nextIndex = (int)Random(start, end.Value);
                if (start != nextIndex)
                {
                    (items[start], items[nextIndex]) = (items[nextIndex], items[start]);
                }
                start++;
            }
        }

        public static PrngSeed GenerateSeed()
        {
            return ConvertSeed(SodiumRng.GenerateSeed());
        }

        public static PrngSeed ConvertSeed(SodiumRngSeed seed)
        {
            return new PrngSeed($"{seed.Type},{seed.HexString}");
        }

        public static PrngSeed ConvertSeed(Gen5RngSeed seed)
        {
            int[] values = seed.ToArray();
            return new PrngSeed(string.Join(",", values));
        }

        public static Prng Get(object? prng = null)
        {
            return prng switch
            {
                Prng p => p,
                PrngSeed seed => new Prng(seed),
                string str => new Prng(new PrngSeed(str)),
                null => new Prng(),
                _ => new Prng()
            };
        }
    }

    /// <summary>
    /// This is a drop-in replacement for libsodium's randombytes_buf_deterministic,
    /// but it's implemented with ChaCha20 instead, for a smaller dependency that
    /// doesn't use NodeJS native modules, for better portability.
    /// </summary>
    public class SodiumRng : IRng
    {
        // Nonce chosen to be compatible with libsodium's randombytes_buf_deterministic
        // https://github.com/jedisct1/libsodium/blob/ce07d6c82c0e6c75031cf627913bf4f9d3f1e754/src/libsodium/randombytes/randombytes.c#L178
        private static readonly byte[] Nonce = "LibsodiumDRG"u8.ToArray();

        private byte[] seed = new byte[32];

        /// <summary>Creates a new source of randomness for the given seed.</summary>
        public SodiumRng(SodiumRngSeed seed)
        {
            SetSeed(seed);
        }

        public void SetSeed(SodiumRngSeed seedData)
        {
            // randombytes_buf_deterministic requires 32 bytes, but
            // generateSeed generates 16 bytes, so the last 16 bytes will be 0
            // when starting out. This shouldn't cause any problems.
            byte[] seedBuf = new byte[32];
            string hexString = seedData.HexString.PadRight(64, '0');

            for (int i = 0; i < hexString.Length; i += 2)
            {
                seedBuf[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }

            seed = seedBuf;
        }

        public PrngSeed GetSeed()
        {
            string hexString = Convert.ToHexString(seed).ToLowerInvariant();
            return new PrngSeed($"sodium,{hexString}");
        }

        public uint Next()
        {
            byte[] zeroBuf = new byte[36];

            // Use ChaCha20 encryption - tested to do the exact same thing as
            // sodium.randombytes_buf_deterministic(buf, this.seed);
            byte[] buf = ChaCha20.Encrypt(seed, Nonce, zeroBuf);

            // Use the first 32 bytes for the next seed, and the next 4 bytes for the output
            seed = buf[0..32];

            // Reading big-endian
            return (uint)((buf[32] << 24) | (buf[33] << 16) | (buf[34] << 8) | buf[35]);
        }

        public static SodiumRngSeed GenerateSeed()
        {
            byte[] seed = new byte[16];
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(seed);

            string hexString = Convert.ToHexString(seed).ToLowerInvariant();
            return new SodiumRngSeed("sodium", hexString);
        }
    }

    /// <summary>
    /// A PRNG intended to emulate the on-cartridge PRNG for Gen 5 with a 64-bit initial seed.
    /// </summary>
    public class Gen5Rng : IRng
    {
        private Gen5RngSeed seed;

        /// <summary>Creates a new source of randomness for the given seed.</summary>
        public Gen5Rng(Gen5RngSeed? seed = null)
        {
            this.seed = seed ?? GenerateSeed();
        }

        public PrngSeed GetSeed()
        {
            int[] values = seed.ToArray();
            return new PrngSeed(string.Join(",", values));
        }

        public uint Next()
        {
            seed = NextFrame(seed); // Advance the RNG
            return (uint)((seed.Value0 << 16) + seed.Value1); // Use the upper 32 bits
        }

        /// <summary>
        /// Calculates a * b + c (with 64-bit 2's complement integers)
        /// </summary>
        private static Gen5RngSeed MultiplyAdd(Gen5RngSeed a, Gen5RngSeed b, Gen5RngSeed c)
        {
            // If you've done long multiplication, this is the same thing.
            int[] aArray = a.ToArray();
            int[] bArray = b.ToArray();
            int[] cArray = c.ToArray();
            int[] outArray = new int[4];
            long carry = 0;

            for (int outIndex = 3; outIndex >= 0; outIndex--)
            {
                for (int bIndex = outIndex; bIndex < 4; bIndex++)
                {
                    int aIndex = 3 - (bIndex - outIndex);
                    carry += (long)aArray[aIndex] * bArray[bIndex];
                }
                carry += cArray[outIndex];

                outArray[outIndex] = (int)(carry & 0xFFFF);
                carry >>= 16;
            }

            return Gen5RngSeed.FromArray(outArray);
        }

        /// <summary>
        /// The RNG is a Linear Congruential Generator (LCG) in the form: x_{n + 1} = (a x_n + c) % m
        /// 
        /// Where: x_0 is the seed, x_n is the random number after n iterations,
        /// 
        /// a = 0x5D588B656C078965
        /// c = 0x00269EC3
        /// m = 2^64
        /// </summary>
        private static Gen5RngSeed NextFrame(Gen5RngSeed seed, int framesToAdvance = 1)
        {
            Gen5RngSeed a = new Gen5RngSeed(0x5D58, 0x8B65, 0x6C07, 0x8965);
            Gen5RngSeed c = new Gen5RngSeed(0, 0, 0x26, 0x9EC3);

            for (int i = 0; i < framesToAdvance; i++)
            {
                // seed = seed * a + c
                seed = MultiplyAdd(seed, a, c);
            }

            return seed;
        }

        public static Gen5RngSeed GenerateSeed()
        {
            Random random = new Random();
            return new Gen5RngSeed(
                random.Next(0, 65536), // 2^16
                random.Next(0, 65536),
                random.Next(0, 65536),
                random.Next(0, 65536)
            );
        }
    }
}