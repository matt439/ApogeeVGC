namespace ApogeeVGC_CS.sim
{
    public class PokemonSet
    {
        /**
         * Nickname. Should be identical to its base species if not specified
         * by the player, e.g. "Minior".
         */
        public required string Name { get; set; }

        /**
         * Species name (including forme if applicable), e.g. "Minior-Red".
         * This should always be converted to an id before use.
         */
        public required string Species { get; init; }

        /**
         * This can be an id, e.g. "whiteherb" or a full name, e.g. "White Herb".
         * This should always be converted to an id before use.
         */
        public required string Item { get; init; }

        /**
         * This can be an id, e.g. "shieldsdown" or a full name,
         * e.g. "Shields Down".
         * This should always be converted to an id before use.
         */
        public required string Ability { get; init; }

        /**
         * Each move can be an id, e.g. "shellsmash" or a full name,
         * e.g. "Shell Smash"
         * These should always be converted to ids before use.
         */
        public required List<string> Moves { get; init; }

        /**
        * This can be an id, e.g. "adamant" or a full name, e.g. "Adamant".
        * This should always be converted to an id before use.
        */
        public required string Nature { get; init; }

        public required GenderName Gender { get; init; }

        /**
         * Effort Values, used in stat calculation.
         * These must be between 0 and 255, inclusive.
         *
         * Also used to store AVs for Let's Go
         */
        public required StatsTable Evs
        {
            get;
            set
            {
                if (!value.IsValidEvs())
                {
                    throw new ArgumentException("EVs must be between 0 and 255, inclusive.");
                }
                field = value;
            }
        }

        /**
         * Individual Values, used in stat calculation.
         * These must be between 0 and 31, inclusive.
         *
         * These are also used as DVs, or determinant values, in Gens
         * 1 and 2, which are represented as even numbers from 0 to 30.
         *
         * In Gen 2-6, these must match the Hidden Power type.
         *
         * In Gen 7+, Bottle Caps means these can either match the
         * Hidden Power type or 31.
         */
        public StatsTable Ivs
        {
            get;
            set
            {
                if (!value.IsValidIvs())
                {
                    throw new ArgumentException("IVs must be between 0 and 31, inclusive.");
                }
                field = value;
            }
        } = StatsTable.PerfectIvs;

        /**
         * This is usually between 1 and 100, inclusive,
         * but the simulator supports levels up to 9999 for testing purposes.
         */
        public required int Level
        {
            get;
            init
            {
                if (value is < 1 or > 9999)
                {
                    throw new ArgumentException("Level must be between 1 and 9999, inclusive.");
                }
                field = value;
            }
        }

        /**
         * While having no direct competitive effect, certain Pokemon cannot
         * be legally obtained as shiny, either as a whole or with certain
         * event-only abilities or moves.
         */
        public bool? Shiny { get; init; }

        /**
         * This is technically "Friendship", but the community calls this
         * "Happiness".
         *
         * It's used to calculate the power of the moves Return and Frustration.
         * This value must be between 0 and 255, inclusive.
         */
        public int? Happiness
        {
            get;
            init
            {
                if (value is < 0 or > 255)
                {
                    throw new ArgumentException("Happiness must be between 0 and 255, inclusive.");
                }
                field = value;
            }
        }

        /**
         * The pokeball this Pokemon is in. Like shininess, this property
         * has no direct competitive effects, but has implications for
         * event legality. For example, any Rayquaza that knows V-Create
         * must be sent out from a Cherish Ball.
         */
        public string? Pokeball { get; init; }

        /**
         * Hidden Power type. Optional in older gens, but used in Gen 7+
         * because `ivs` contain post-Battle-Cap values.
         */
        public PokemonType? HpType { get; set; }

        /**
         * Dynamax Level. Affects the amount of HP gained when Dynamaxed.
         * This value must be between 0 and 10, inclusive.
         */
        public int? DynamaxLevel
        {
            get;
            init
            {
                if (value is < 0 or > 10)
                {
                    throw new ArgumentException("Dynamax Level must be between 0 and 10, inclusive.");
                }
                field = value;
            }
        }

        public bool? Gigantamax { get; init; }

        public PokemonType? TeraType { get; init; }

        public int? AdjustLevel { get; init; } // added for Pokemon class constructor

        //public PokemonSet(SpeciesData data, string item, string ability, List<string> moves, string nature,
        //    GenderName gender, StatsTable evs, StatsTable ivs, int level)
        //{
        //    Name = data.Name;
        //    Species = data.Id.ToString();
        //    Item = item;
        //    Ability = ability;
        //    Moves = moves;
        //    Nature = nature;
        //    Gender = gender;
        //    Evs = evs;
        //    Ivs = ivs;
        //    Level = level;
        //}
    }

    public static class Teams
    {
        public static string Pack(List<PokemonSet>? team = null)
        {
            throw new NotImplementedException();
        }

        public static List<PokemonSet>? Unpack(string buf)
        {
            throw new NotImplementedException();
        }

        public static string PackName(string? name = null)
        {
            throw new NotImplementedException();
        }

        public static string? UnpackName(string? name, Func<string, AnyObject>? dexTable = null)
        {
            throw new NotImplementedException();
        }

        public static string Export(List<PokemonSet> team, object? options = null)
        {
            throw new NotImplementedException();
        }

        public static string ExportSet(PokemonSet set, object? options = null)
        {
            throw new NotImplementedException();
        }

        public static void ParseExportedTeamLine(string line, bool isFirstLine, PokemonSet set, bool? aggressive = null)
        {
            throw new NotImplementedException();
        }

        public static List<PokemonSet>? Import(string buffer, bool? aggressive = null)
        {
            throw new NotImplementedException();
        }

        public static object GetGenerator(string format, Prng seed)
        {
            throw new NotImplementedException();
        }

        public static object GetGenerator(Format format, Prng seed)
        {
            throw new NotImplementedException();
        }

        public static object GetGenerator(string format, PrngSeed seed)
        {
            throw new NotImplementedException();
        }

        public static object GetGenerator(Format format, PrngSeed seed)
        {
            throw new NotImplementedException();
        }

        public static object GetGenerator(string format)
        {
            throw new NotImplementedException();
        }

        public static object GetGenerator(Format format)
        {
            throw new NotImplementedException();
        }

        public static List<PokemonSet> Generate(string format, PlayerOptions? options = null)
        {
            throw new NotImplementedException();
        }

        public static List<PokemonSet> Generate(Format format, PlayerOptions? options = null)
        {
            throw new NotImplementedException();
        }
    }
}