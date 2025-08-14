using ApogeeVGC_CS.sim;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    public class TeamGenerator
    {
        public required PrngSeed Seed { get; set; }

        public List<PokemonSet> GetTeam(PlayerOptions options)
        {
            throw new NotImplementedException("Team generation logic is not implemented yet.");
        }

    //public List<PokemonSet> GetTeam(PlayerOptions options)
        //{
        //    // Initialize PRNG with the seed for deterministic generation
        //    var prng = new Prng(Seed);

        //    // Get the format information to determine team generation rules
        //    // This would typically come from options or be passed to the constructor
        //    var format = GetFormat(options); // You'll need to implement this helper

        //    // Determine team size based on format rules
        //    int teamSize = GetTeamSize(format);

        //    var team = new List<PokemonSet>();

        //    // Generate each team member
        //    for (int i = 0; i < teamSize; i++)
        //    {
        //        PokemonSet pokemon = GenerateRandomPokemon(prng, format, options);
        //        team.Add(pokemon);
        //    }

        //    // Apply any format-specific post-processing
        //    ApplyFormatRules(team, format, options);

        //    return team;
        //}

        //// Helper method to generate a single random Pokémon
        //private PokemonSet GenerateRandomPokemon(Prng prng, Format format, PlayerOptions options)
        //{
        //    // Get available species for this format
        //    var availableSpecies = GetAvailableSpecies(format);

        //    // Randomly select a species
        //    var selectedSpecies = prng.Sample(availableSpecies);

        //    // Generate random stats and moves
        //    var moves = GenerateRandomMoves(selectedSpecies, prng, format);
        //    var nature = GenerateRandomNature(prng);
        //    var ability = GenerateRandomAbility(selectedSpecies, prng);
        //    var item = GenerateRandomItem(prng, format);
        //    var evs = GenerateRandomEvs(prng, format);
        //    var ivs = GenerateRandomIvs(prng, format);

        //    return new PokemonSet
        //    {
        //        Name = selectedSpecies.Name,
        //        Species = selectedSpecies.Id.ToString(),
        //        Item = item,
        //        Ability = ability,
        //        Moves = moves,
        //        Nature = nature,
        //        Gender = GenerateRandomGender(selectedSpecies, prng),
        //        Evs = evs,
        //        Ivs = ivs,
        //        Level = GetStandardLevel(format),
        //        Shiny = prng.RandomChance(1, 1024), // Standard shiny rate
        //        Happiness = 255,
        //        TeraType = GenerateRandomTeraType(selectedSpecies, prng, format)
        //    };
        //}

        //// Helper methods you'll need to implement:

        //private Format GetFormat(PlayerOptions options)
        //{
        //    // Extract format from options or use a default
        //    // This might require access to a Dex or Format registry
        //    throw new NotImplementedException("Need access to format data");
        //}

        //private int GetTeamSize(Format format)
        //{
        //    // Most formats use 6 Pokémon, but some might differ
        //    return format.PlayerCount == 2 ? 6 : 4; // Example logic
        //}

        //private List<Species> GetAvailableSpecies(Format format)
        //{
        //    // Get list of species allowed in this format
        //    // This would typically filter based on format rules
        //    throw new NotImplementedException("Need access to species data");
        //}

        //private List<string> GenerateRandomMoves(Species species, Prng prng, Format format)
        //{
        //    // Generate 4 random moves from the species' movepool
        //    var availableMoves = GetSpeciesMoveset(species, format);
        //    var selectedMoves = new List<string>();

        //    // Ensure we don't pick duplicate moves
        //    var shuffledMoves = prng.Shuffle(availableMoves.ToList());
        //    return shuffledMoves.Take(4).ToList();
        //}

        //private string GenerateRandomNature(Prng prng)
        //{
        //    var natures = new[] { "Hardy", "Adamant", "Modest", "Jolly", "Timid", /* ... all natures */ };
        //    return prng.Sample(natures);
        //}

        //private string GenerateRandomAbility(Species species, Prng prng)
        //{
        //    // Pick from the species' available abilities
        //    var abilities = GetSpeciesAbilities(species);
        //    return prng.Sample(abilities);
        //}

        //private string GenerateRandomItem(Prng prng, Format format)
        //{
        //    // Generate appropriate item based on format rules
        //    var items = GetFormatItems(format);
        //    return prng.Sample(items);
        //}

        //private StatsTable GenerateRandomEvs(Prng prng, Format format)
        //{
        //    // Random Battles typically use optimized EV spreads
        //    // This is a simplified example - real implementation would be more sophisticated
        //    return new StatsTable
        //    {
        //        Hp = 0,
        //        Atk = prng.Next(0, 253),
        //        Def = prng.Next(0, 253),
        //        Spa = prng.Next(0, 253),
        //        Spd = prng.Next(0, 253),
        //        Spe = prng.Next(0, 253)
        //    };
        //}

        //private StatsTable GenerateRandomIvs(Prng prng, Format format)
        //{
        //    // Most competitive formats use perfect IVs
        //    return StatsTable.PerfectIvs;
        //}

        //private GenderName GenerateRandomGender(Species species, Prng prng)
        //{
        //    // Check species gender ratio and generate accordingly
        //    // Simplified - real implementation would use species data
        //    return prng.RandomChance(1, 2) ? GenderName.Male : GenderName.Female;
        //}

        //private int GetStandardLevel(Format format)
        //{
        //    // Most formats use level 50 or 100
        //    return format.Name.Contains("VGC") ? 50 : 100;
        //}

        //private PokemonType? GenerateRandomTeraType(Species species, Prng prng, Format format)
        //{
        //    // Generate tera type if format supports it
        //    if (!format.Name.Contains("Gen9")) return null;

        //    var allTypes = Enum.GetValues<PokemonType>();
        //    return prng.Sample(allTypes);
        //}
    }

    public static class Teams
    {
        public static string Pack(List<PokemonSet>? team = null)
        {
            if (team == null || team.Count == 0) return string.Empty;

            // Helper function to get IV value (empty string if 31 or default)
            string GetIv(StatsTable ivs, StatId stat)
            {
                int value = ivs.GetStat(stat);
                return value == 31 ? string.Empty : value.ToString();
            }

            StringBuilder buf = new StringBuilder();

            foreach (PokemonSet set in team)
            {
                if (buf.Length > 0) buf.Append(']');

                // name
                buf.Append(set.Name ?? set.Species);

                // species
                string id = PackName(set.Species ?? set.Name);
                string nameId = PackName(set.Name ?? set.Species);
                buf.Append($"|{(nameId == id ? string.Empty : id)}");

                // item
                buf.Append($"|{PackName(set.Item)}");

                // ability
                buf.Append($"|{PackName(set.Ability)}");

                // moves
                buf.Append("|");
                buf.Append(string.Join(",", set.Moves.Select(PackName)));

                // nature
                buf.Append($"|{set.Nature ?? string.Empty}");

                // evs
                string evs = "|";
                if (set.Evs != null)
                {
                    string[] evValues =
                    [
                        set.Evs.Hp == 0 ? string.Empty : set.Evs.Hp.ToString(),
                        set.Evs.Atk == 0 ? string.Empty : set.Evs.Atk.ToString(),
                        set.Evs.Def == 0 ? string.Empty : set.Evs.Def.ToString(),
                        set.Evs.Spa == 0 ? string.Empty : set.Evs.Spa.ToString(),
                        set.Evs.Spd == 0 ? string.Empty : set.Evs.Spd.ToString(),
                        set.Evs.Spe == 0 ? string.Empty : set.Evs.Spe.ToString()
                    ];
                    evs = $"|{string.Join(",", evValues)}";
                }

                if (evs == "|,,,,,")
                {
                    buf.Append("|");
                }
                else
                {
                    buf.Append(evs);
                }

                // gender
                if (set.Gender != GenderName.Empty && set.Gender != default(GenderName))
                {
                    buf.Append($"|{set.Gender}");
                }
                else
                {
                    buf.Append("|");
                }

                // ivs
                string ivs = "|";
                if (set.Ivs != null)
                {
                    string[] ivValues = new[]
                    {
                GetIv(set.Ivs, StatId.Hp),
                GetIv(set.Ivs, StatId.Atk),
                GetIv(set.Ivs, StatId.Def),
                GetIv(set.Ivs, StatId.Spa),
                GetIv(set.Ivs, StatId.Spd),
                GetIv(set.Ivs, StatId.Spe)
            };
                    ivs = $"|{string.Join(",", ivValues)}";
                }

                if (ivs == "|,,,,,")
                {
                    buf.Append("|");
                }
                else
                {
                    buf.Append(ivs);
                }

                // shiny
                if (set.Shiny == true)
                {
                    buf.Append("|S");
                }
                else
                {
                    buf.Append("|");
                }

                // level
                if (set.Level != 100)
                {
                    buf.Append($"|{set.Level}");
                }
                else
                {
                    buf.Append("|");
                }

                // happiness
                if (set.Happiness.HasValue && set.Happiness.Value != 255)
                {
                    buf.Append($"|{set.Happiness.Value}");
                }
                else
                {
                    buf.Append("|");
                }

                // Extended data (comma-separated after happiness)
                bool hasExtendedData = !string.IsNullOrEmpty(set.Pokeball) ||
                                      set.HpType.HasValue ||
                                      set.Gigantamax == true ||
                                      (set.DynamaxLevel.HasValue && set.DynamaxLevel.Value != 10) ||
                                      set.TeraType.HasValue;

                if (hasExtendedData)
                {
                    // HP Type
                    buf.Append($",{set.HpType?.ToString() ?? string.Empty}");

                    // Pokeball
                    buf.Append($",{PackName(set.Pokeball ?? string.Empty)}");

                    // Gigantamax
                    buf.Append($",{(set.Gigantamax == true ? "G" : string.Empty)}");

                    // Dynamax Level
                    string dynamaxLevel = string.Empty;
                    if (set.DynamaxLevel.HasValue && set.DynamaxLevel.Value != 10)
                    {
                        dynamaxLevel = set.DynamaxLevel.Value.ToString();
                    }
                    buf.Append($",{dynamaxLevel}");

                    // Tera Type
                    buf.Append($",{set.TeraType?.ToString() ?? string.Empty}");
                }
            }

            return buf.ToString();
        }

        public static List<PokemonSet>? Unpack(string buf)
        {
            if (string.IsNullOrEmpty(buf)) return null;

            // Handle JSON array format
            if (buf.StartsWith('[') && buf.EndsWith(']'))
            {
                try
                {
                    // Parse JSON and re-pack it (assuming JSON contains PokemonSet array)
                    var jsonTeam = JsonSerializer.Deserialize<List<PokemonSet>>(buf);
                    if (jsonTeam != null)
                    {
                        buf = Pack(jsonTeam);
                    }
                }
                catch
                {
                    return null;
                }
            }

            var team = new List<PokemonSet>();
            int i = 0;
            int j = 0;

            // Limit to 24 Pokemon
            for (int count = 0; count < 24; count++)
            {
                // Helper function to find next delimiter
                int FindNext(char delimiter)
                {
                    int pos = buf.IndexOf(delimiter, i);
                    return pos;
                }

                // Helper function to extract substring
                string Extract()
                {
                    string result = buf.Substring(i, j - i);
                    i = j + 1;
                    return result;
                }

                // name
                j = FindNext('|');
                if (j < 0) return null;
                string name = Extract();

                // species  
                j = FindNext('|');
                if (j < 0) return null;
                string speciesStr = Extract();
                string species = UnpackSpeciesName(speciesStr) ?? name;

                // item
                j = FindNext('|');
                if (j < 0) return null;
                string itemStr = Extract();
                string item = UnpackItemName(itemStr);

                // ability
                j = FindNext('|');
                if (j < 0) return null;
                string abilityStr = Extract();
                string ability = ParseAbility(abilityStr, species);

                // moves
                j = FindNext('|');
                if (j < 0) return null;
                string movesStr = Extract();
                var moves = movesStr.Split(',', 24)
                    .Select(UnpackMoveName)
                    .Where(move => !string.IsNullOrEmpty(move))
                    .ToList();

                // nature
                j = FindNext('|');
                if (j < 0) return null;
                string natureStr = Extract();
                string nature = UnpackNatureName(natureStr);

                // evs
                j = FindNext('|');
                if (j < 0) return null;
                var evs = new StatsTable();
                if (j != i)
                {
                    string evsStr = Extract();
                    string[] evArray = evsStr.Split(',', 6);
                    evs = new StatsTable
                    {
                        Hp = ParseInt(evArray.ElementAtOrDefault(0)) ?? 0,
                        Atk = ParseInt(evArray.ElementAtOrDefault(1)) ?? 0,
                        Def = ParseInt(evArray.ElementAtOrDefault(2)) ?? 0,
                        Spa = ParseInt(evArray.ElementAtOrDefault(3)) ?? 0,
                        Spd = ParseInt(evArray.ElementAtOrDefault(4)) ?? 0,
                        Spe = ParseInt(evArray.ElementAtOrDefault(5)) ?? 0
                    };
                }
                else
                {
                    i = j + 1;
                }

                // gender
                j = FindNext('|');
                if (j < 0) return null;
                var gender = GenderName.Empty;
                if (i != j)
                {
                    string genderStr = Extract();
                    gender = ParseGender(genderStr);
                }
                else
                {
                    i = j + 1;
                }

                // ivs
                j = FindNext('|');
                if (j < 0) return null;
                StatsTable ivs = StatsTable.PerfectIvs;
                if (j != i)
                {
                    string ivsStr = Extract();
                    string[] ivArray = ivsStr.Split(',', 6);
                    ivs = new StatsTable
                    {
                        Hp = ParseIv(ivArray.ElementAtOrDefault(0)),
                        Atk = ParseIv(ivArray.ElementAtOrDefault(1)),
                        Def = ParseIv(ivArray.ElementAtOrDefault(2)),
                        Spa = ParseIv(ivArray.ElementAtOrDefault(3)),
                        Spd = ParseIv(ivArray.ElementAtOrDefault(4)),
                        Spe = ParseIv(ivArray.ElementAtOrDefault(5))
                    };
                }
                else
                {
                    i = j + 1;
                }

                // shiny
                j = FindNext('|');
                if (j < 0) return null;
                bool shiny = i != j;
                if (i != j) Extract(); else i = j + 1;

                // level
                j = FindNext('|');
                if (j < 0) return null;
                int level = 100;
                if (i != j)
                {
                    string levelStr = Extract();
                    level = int.TryParse(levelStr, out int parsedLevel) ? parsedLevel : 100;
                }
                else
                {
                    i = j + 1;
                }

                // happiness and extended data
                j = buf.IndexOf(']', i);
                string[]? misc = null;
                if (j < 0)
                {
                    if (i < buf.Length)
                    {
                        misc = buf.Substring(i).Split(',', 6);
                    }
                }
                else
                {
                    if (i != j)
                    {
                        misc = buf.Substring(i, j - i).Split(',', 6);
                    }
                }

                // Parse extended data
                int? happiness = null;
                PokemonType? hpType = null;
                string? pokeball = null;
                bool? gigantamax = null;
                int? dynamaxLevel = null;
                PokemonType? teraType = null;

                if (misc != null)
                {
                    happiness = ParseInt(misc.ElementAtOrDefault(0)) ?? 255;
                    hpType = ParsePokemonType(misc.ElementAtOrDefault(1));
                    pokeball = UnpackItemName(misc.ElementAtOrDefault(2));
                    gigantamax = !string.IsNullOrEmpty(misc.ElementAtOrDefault(3));
                    dynamaxLevel = ParseInt(misc.ElementAtOrDefault(4)) ?? 10;
                    teraType = ParsePokemonType(misc.ElementAtOrDefault(5));
                }

                // Create PokemonSet
                var pokemonSet = new PokemonSet
                {
                    Name = name,
                    Species = species,
                    Item = item,
                    Ability = ability,
                    Moves = moves,
                    Nature = nature,
                    Gender = gender,
                    Evs = evs,
                    Ivs = ivs,
                    Level = level,
                    Shiny = shiny ? true : null,
                    Happiness = happiness,
                    Pokeball = pokeball,
                    HpType = hpType,
                    Gigantamax = gigantamax,
                    DynamaxLevel = dynamaxLevel,
                    TeraType = teraType
                };

                team.Add(pokemonSet);

                if (j < 0) break;
                i = j + 1;
            }

            return team;
        }

        // Helper methods
        private static string ParseAbility(string abilityStr, string species)
        {
            // Handle special ability codes
            if (string.IsNullOrEmpty(abilityStr) ||
                new[] { "", "0", "1", "H", "S" }.Contains(abilityStr))
            {
                // TODO: Get species data and return appropriate ability
                // For now, return error placeholder or empty
                return abilityStr == "" ? string.Empty : "!!!ERROR!!!";
            }

            return UnpackAbilityName(abilityStr);
        }

        private static int? ParseInt(string? value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return int.TryParse(value, out int result) ? result : null;
        }

        private static int ParseIv(string? value)
        {
            if (string.IsNullOrEmpty(value)) return 31; // Default IV
            return int.TryParse(value, out int result) ? result : 0;
        }

        private static GenderName ParseGender(string genderStr)
        {
            return genderStr.ToUpperInvariant() switch
            {
                "M" => GenderName.M,
                "F" => GenderName.F,
                "N" => GenderName.N,
                _ => GenderName.Empty
            };
        }

        private static PokemonType? ParsePokemonType(string? typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return null;

            // TODO: Parse PokemonType enum from string
            if (Enum.TryParse<PokemonType>(typeStr, true, out PokemonType result))
            {
                return result;
            }
            return null;
        }


        public static string PackName(string? name = null)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;

            // Convert to lowercase and remove non-alphanumeric characters
            // This should match the ID conversion logic used elsewhere in the codebase
            return new string(name.ToLowerInvariant()
                .Where(c => char.IsLetterOrDigit(c))
                .ToArray());
        }

        ///// <summary>
        ///// Will not entirely recover a packed name, but will be a pretty readable guess
        ///// </summary>
        ///// <param name="name">The packed name to unpack</param>
        ///// <param name="dexTable">Optional dex table function</param>
        ///// <returns>A readable name string</returns>
        //public static string UnpackName(string? name, Func<string, AnyObject>? dexTable = null)
        //{
        //    if (string.IsNullOrEmpty(name)) return string.Empty;

        //    // Try to get the name from the dex table if provided
        //    if (dexTable != null)
        //    {
        //        try
        //        {
        //            var obj = dexTable(name);
        //            if (TryGetDexObjectName(obj, out string? dexName))
        //            {
        //                return dexName;
        //            }
        //        }
        //        catch
        //        {
        //            // Fall through to string formatting if dex lookup fails
        //        }
        //    }

        //    return FormatPackedName(name);
        //}

        /// <summary>
        /// Unpack name using Species dex table
        /// </summary>
        public static string? UnpackSpeciesName(string? name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            try
            {
                var dex = Dex.BaseDex; // Assuming static Dex access
                var species = dex.Species.Get(name);
                if (species?.Exists == true)
                {
                    return species.Name;
                }
            }
            catch
            {
                // Fall through to formatting
            }

            return FormatPackedName(name);
        }

        /// <summary>
        /// Unpack name using Items dex table
        /// </summary>
        public static string UnpackItemName(string? name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;

            try
            {
                var dex = Dex.BaseDex;
                var item = dex.Items.Get(name);
                if (item?.Exists == true)
                {
                    return item.Name;
                }
            }
            catch
            {
                // Fall through to formatting
            }

            return FormatPackedName(name);
        }

        /// <summary>
        /// Unpack name using Abilities dex table
        /// </summary>
        public static string UnpackAbilityName(string? name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;

            try
            {
                var dex = Dex.BaseDex;
                var ability = dex.Abilities.Get(name);
                if (ability?.Exists == true)
                {
                    return ability.Name;
                }
            }
            catch
            {
                // Fall through to formatting
            }

            return FormatPackedName(name);
        }

        public static string UnpackMoveName(string? name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            try
            {
                var dex = Dex.BaseDex;
                var move = dex.Moves.Get(name);
                if (move?.Exists == true)
                {
                    return move.Name;
                }
            }
            catch
            {
                // Fall through to formatting
            }
            return FormatPackedName(name);
        }

        public static string UnpackNatureName(string? name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            try
            {
                ModdedDex dex = Dex.BaseDex;
                Nature? nature = dex.Natures.Get(name);
                if (nature?.Exists == true)
                {
                    return nature.Name;
                }
            }
            catch
            {
                // Fall through to formatting
            }
            return FormatPackedName(name);
        }

        /// <summary>
        /// Helper method to extract name from dex object
        /// </summary>
        private static bool TryGetDexObjectName(AnyObject obj, out string? name)
        {
            name = null;

            // Check if object exists
            if (!obj.TryGetValue("exists", out object? existsValue) ||
                existsValue is not bool exists || !exists)
            {
                return false;
            }

            // Try to get name
            if (obj.TryGetValue("name", out object? nameValue) &&
                nameValue is string objName && !string.IsNullOrEmpty(objName))
            {
                name = objName;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Formats a packed name to be more readable
        /// </summary>
        private static string FormatPackedName(string name)
        {
            // Chain the regex replacements for better readability
            return Regex.Replace(
                    Regex.Replace(
                        Regex.Replace(name, @"([0-9]+)", " $1 "),
                        @"([A-Z])", " $1"),
                    @"[ ]{2,}", " ")
                .Trim();
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

        public static TeamGenerator GetGenerator(string format, Prng seed)
        {
            throw new NotImplementedException();
        }

        public static TeamGenerator GetGenerator(Format format, Prng seed)
        {
            throw new NotImplementedException();
        }

        public static TeamGenerator GetGenerator(string format, PrngSeed seed)
        {
            throw new NotImplementedException();
        }

        public static TeamGenerator GetGenerator(Format format, PrngSeed seed)
        {
            throw new NotImplementedException();
        }

        public static TeamGenerator GetGenerator(string format)
        {
            throw new NotImplementedException();
        }

        public static TeamGenerator GetGenerator(Format format)
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