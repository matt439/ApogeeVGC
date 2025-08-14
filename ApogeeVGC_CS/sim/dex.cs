using Microsoft.VisualBasic;
using System.Text.Json;

namespace ApogeeVGC_CS.sim
{
    //public class Dex
    //{
    //    public Dictionary<string, ModdedDex> Dexes { get; init; } = [];

    //    public Dex()
    //    {
    //        var baseDex = new ModdedDex();
    //        Dexes[baseDex.CurrentMod] = baseDex;
    //        Dexes[DexConstants.BaseMod.ToString()] = baseDex;
    //    }
    //}

    public static class Dex
    {
        public static readonly Dictionary<string, ModdedDex> Dexes = [];
        private static readonly Lock Lock = new();

        static Dex()
        {
            // Initialize with base dex
            var baseDex = new ModdedDex();
            RegisterDex(baseDex.CurrentMod, baseDex);
            RegisterDex(DexConstants.BaseMod.ToString(), baseDex);

            GetRequiredDex("base").LoadTestData();
            GetRequiredDex(DexConstants.BaseMod.ToString()).LoadTestData();
        }

        /// <summary>
        /// Registers a ModdedDex with the given key
        /// </summary>
        /// <param name="key">The key to register the dex under</param>
        /// <param name="dex">The ModdedDex to register</param>
        public static void RegisterDex(string key, ModdedDex dex)
        {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(dex);

            lock (Lock)
            {
                Dexes[key] = dex;
            }
        }

        /// <summary>
        /// Gets a ModdedDex by key
        /// </summary>
        /// <param name="key">The key to look up</param>
        /// <returns>The ModdedDex if found, null otherwise</returns>
        public static ModdedDex? GetDex(string key)
        {
            ArgumentNullException.ThrowIfNull(key);

            lock (Lock)
            {
                return Dexes.GetValueOrDefault(key);
            }
        }

        /// <summary>
        /// Gets a ModdedDex by key, throwing if not found
        /// </summary>
        /// <param name="key">The key to look up</param>
        /// <returns>The ModdedDex</returns>
        /// <exception cref="KeyNotFoundException">Thrown when key is not found</exception>
        public static ModdedDex GetRequiredDex(string key)
        {
            ModdedDex? dex = GetDex(key);
            return dex ?? throw new KeyNotFoundException($"Dex with key '{key}' not found");
        }

        /// <summary>
        /// Checks if a dex is registered with the given key
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if registered, false otherwise</returns>
        public static bool HasDex(string key)
        {
            ArgumentNullException.ThrowIfNull(key);

            lock (Lock)
            {
                return Dexes.ContainsKey(key);
            }
        }

        /// <summary>
        /// Gets all registered dex keys
        /// </summary>
        /// <returns>Collection of all registered keys</returns>
        public static IReadOnlyCollection<string> GetRegisteredKeys()
        {
            lock (Lock)
            {
                return Dexes.Keys.ToArray();
            }
        }

        /// <summary>
        /// Gets all registered dexes
        /// </summary>
        /// <returns>Dictionary of all registered dexes</returns>
        public static IReadOnlyDictionary<string, ModdedDex> GetAllDexes()
        {
            lock (Lock)
            {
                return new Dictionary<string, ModdedDex>(Dexes);
            }
        }

        /// <summary>
        /// Removes a dex registration
        /// </summary>
        /// <param name="key">The key to remove</param>
        /// <returns>True if removed, false if not found</returns>
        public static bool UnregisterDex(string key)
        {
            ArgumentNullException.ThrowIfNull(key);

            lock (Lock)
            {
                return Dexes.Remove(key);
            }
        }

        /// <summary>
        /// Gets the base dex
        /// </summary>
        public static ModdedDex BaseDex => GetRequiredDex(DexConstants.BaseMod.ToString());

        /// <summary>
        /// Gets or creates a dex for the specified mod
        /// </summary>
        /// <param name="mod">The mod identifier</param>
        /// <returns>The ModdedDex for the mod</returns>
        public static ModdedDex ForMod(string mod)
        {
            ArgumentNullException.ThrowIfNull(mod);

            var existing = GetDex(mod);
            if (existing != null)
            {
                return existing;
            }

            // Create new dex for the mod
            var newDex = new ModdedDex(mod);
            RegisterDex(mod, newDex);
            return newDex;
        }

        /// <summary>
        /// Clears all registered dexes (useful for testing)
        /// </summary>
        public static void Clear()
        {
            lock (Lock)
            {
                Dexes.Clear();
            }
        }
    }

    public enum DataType
    {
        Abilities,
        Rulesets,
        FormatsData,
        Items,
        Learnsets,
        Moves,
        Natures,
        Pokedex,
        Scripts,
        Conditions,
        TypeChart,
        PokemonGoData
    }

    // Data file mapping
    public static class DataFiles
    {
        public static readonly Dictionary<DataType, string> Files = new()
        {
            { DataType.Abilities, "abilities" },
            { DataType.Rulesets, "rulesets" },
            { DataType.FormatsData, "formats-data" },
            { DataType.Items, "items" },
            { DataType.Learnsets, "learnsets" },
            { DataType.Moves, "moves" },
            { DataType.Natures, "natures" },
            { DataType.Pokedex, "pokedex" },
            { DataType.PokemonGoData, "pokemongo" },
            { DataType.Scripts, "scripts" },
            { DataType.Conditions, "conditions" },
            { DataType.TypeChart, "typechart" }
        };
    }

    public class DexTable<T> : Dictionary<Id, T>;

    public class AliasesTable : Dictionary<Id, string>;

    public class DexTableData
    {
        public DexTable<AbilityData> Abilities { get; init; } = [];
        public DexTable<FormatData> Rulesets { get; init; } = [];
        public DexTable<ItemData> Items { get; init; } = [];
        public DexTable<LearnsetData> Learnsets { get; init; } = [];
        public DexTable<MoveData> Moves { get; init; } = [];
        public DexTable<NatureData> Natures { get; init; } = [];
        public DexTable<SpeciesData> Pokedex { get; init; } = [];
        public DexTable<SpeciesFormatsData> FormatsData { get; init; } = []; 
        public DexTable<PokemonGoData> PokemonGoData { get; init; } = [];
        public DexTable<AnyObject> Scripts { get; init; } = [];
        public DexTable<IConditionData> Conditions { get; init; } = [];
        public DexTable<TypeData> TypeChart { get; init; } = [];
    }

    public class TextTableData
    {
        public DexTable<AbilityText> Abilities { get; set; } = [];
        public DexTable<ItemText> Items { get; set; } = [];
        public DexTable<MoveText> Moves { get; set; } = [];
        public DexTable<PokedexText> Pokedex { get; set; } = [];
        public DexTable<DefaultText> Default { get; set; } = [];
    }

    // Helper class for to hold Dex constants
    public static class DexConstants
    {
        public static Id BaseMod => new("gen9");
        public const string DataDir = "data";
        public const string ModsDir = "mods";
    }

    // Helper struct for GetDescs()
    public struct Descriptions
    {
        public string Desc { get; init; }
        public string ShortDesc { get; init; }
    }

    // Helper struct for GetHiddenPower()
    public struct HiddenPower
    {
        public PokemonType Type { get; init; }
        public int Power { get; init; }
    }

    // Helper enum for DataSearch()
    public enum DataSearchType
    {
        Pokedex,
        Moves,
        Abilities,
        Items,
        Natures,
        TypeChart,
    }

    /// <summary>
    /// MoveText | ItemText | AbilityText | PokedexText | DefaultText
    /// </summary>
    public interface ITextFile;

    public class ModdedDex
    {
        public static string Name => "[ModdedDex]";
        public bool IsBase => CurrentMod == "base";
        public string CurrentMod { get; init; }
        public string DataDir => IsBase ? DexConstants.DataDir : $"{DexConstants.ModsDir}/{CurrentMod}";
        public int Gen { get; set; }
        public string ParentMod { get; init; } = string.Empty;
        public bool ModsLoaded { get; set; }

        public DexTableData? DataCache { get; set; }
        public TextTableData? TextCache { get; set; }

        // Managers for each data type
        public DexFormats Formats { get; }
        public DexAbilities Abilities { get; }
        public DexItems Items { get; }
        public DexMoves Moves { get; }
        public DexSpecies Species { get; }
        public DexConditions Conditions { get; }
        public DexNatures Natures { get; }
        public DexTypes Types { get; }
        public DexStats Stats { get; }

        public Dictionary<Id, Id>? Aliases { get; set; }
        public Dictionary<Id, List<Id>>? FuzzyAliases { get; set; }

        public ModdedDex(string mod = "base")
        {
            CurrentMod = mod;

            Formats = new DexFormats(this);
            Abilities = new DexAbilities(this);
            Items = new DexItems(this);
            Moves = new DexMoves(this);
            Species = new DexSpecies(this);
            Conditions = new DexConditions(this);
            Natures = new DexNatures(this);
            Types = new DexTypes(this);
            Stats = new DexStats(this);
        }

        public DexTableData Data => LoadData();

        public void LoadTestData()
        {
            Formats.LoadTestFormats();
            Abilities.LoadTestAbilities();
            Items.LoadTestItems();
            Moves.LoadTestMoves();
            Species.LoadTestSpecies();
            Conditions.LoadTestConditions();
            Natures.LoadTestNatures();
            Types.LoadTestTypes();
            // Stats seem to already be working
        }

        //public Dictionary<string, ModdedDex> Dexes
        //{
        //    get
        //    {
        //        IncludeMods();
        //        return field;
        //    }
        //}

        public Dictionary<string, ModdedDex> Dexes()
        {
            IncludeMods();
            return Dex.Dexes;
        }

        public ModdedDex Mod(string? mod)
        {
            throw new NotImplementedException("Mod method not implemented yet.");
        }

        public ModdedDex ForGen(int gen)
        {
            throw new NotImplementedException("ForGen method not implemented yet.");
        }

        public ModdedDex ForFormat(string name)
        {
            throw new NotImplementedException("ForFormat method not implemented yet.");
        }

        public ModdedDex ForFormat(Format format)
        {
            throw new NotImplementedException("ForFormat method not implemented yet.");
        }

        public object ModData(DataType dataType, string id)
        {
            throw new NotImplementedException("ModData method not implemented yet.");
        }

        public string EffectToString { get; } = Name;

        /**
         * Sanitizes a username or Pokemon nickname
         *
         * Returns the passed name, sanitized for safe use as a name in the PS
         * protocol.
         *
         * Such a string must uphold these guarantees:
         * - must not contain any ASCII whitespace character other than a space
         * - must not start or end with a space character
         * - must not contain any of: | , [ ]
         * - must not be the empty string
         * - must not contain Unicode RTL control characters
         *
         * If no such string can be found, returns the empty string. Calling
         * functions are expected to check for that condition and deal with it
         * accordingly.
         *
         * getName also enforces that there are not multiple consecutive space
         * characters in the name, although this is not strictly necessary for
         * safety.
         */
        public string GetName(object name)
        {
            throw new NotImplementedException("GetName method not implemented yet.");
        }

        public bool GetImmunity(PokemonType source, List<PokemonType> target)
        {
            throw new NotImplementedException("GetImmunit method not implemented yet.");
        }

        public int GetEffectiveness(PokemonType source, List<PokemonType> target)
        {
            throw new NotImplementedException("GetImmunit method not implemented yet.");
        }

        public Descriptions GetDescs(DexTable<ITextFile> table, Id id, AnyObject dataEntry)
        {
            // Quick path: if dataEntry already has descriptions, use them
            if (dataEntry.TryGetValue("shortDesc", out object? shortDescValue) && shortDescValue is not null)
            {
                string shortDesc = shortDescValue.ToString() ?? "";
                string desc = "";

                if (dataEntry.TryGetValue("desc", out object? descValue) && descValue is not null)
                {
                    desc = descValue.ToString() ?? "";
                }

                return new Descriptions
                {
                    Desc = desc,
                    ShortDesc = shortDesc
                };
            }

            // Load text data and find the entry
            var textData = LoadTextData();
            if (!table.TryGetValue(id, out ITextFile? entry) || entry is null)
            {
                return new Descriptions
                {
                    Desc = "",
                    ShortDesc = ""
                };
            }

            var descs = new Descriptions
            {
                Desc = "",
                ShortDesc = ""
            };

            // Search through generations for descriptions
            int baseGen = Dex.BaseDex.Gen;
            for (int i = Gen; i < baseGen; i++)
            {
                var (currentDesc, currentShortDesc) = GetGenerationDescriptions(entry, i);

                // Fill in missing descriptions
                if (string.IsNullOrEmpty(descs.Desc) && !string.IsNullOrEmpty(currentDesc))
                {
                    descs = descs with { Desc = currentDesc };
                }

                if (string.IsNullOrEmpty(descs.ShortDesc) && !string.IsNullOrEmpty(currentShortDesc))
                {
                    descs = descs with { ShortDesc = currentShortDesc };
                }

                // Break early if both descriptions are found
                if (!string.IsNullOrEmpty(descs.Desc) && !string.IsNullOrEmpty(descs.ShortDesc))
                {
                    break;
                }
            }

            // Apply fallbacks from the entry itself
            if (string.IsNullOrEmpty(descs.ShortDesc))
            {
                descs = descs with { ShortDesc = GetEntryShortDesc(entry) ?? "" };
            }

            if (string.IsNullOrEmpty(descs.Desc))
            {
                string fallbackDesc = GetEntryDesc(entry) ?? descs.ShortDesc;
                descs = descs with { Desc = fallbackDesc };
            }

            return descs;
        }

        // Helper method to get generation-specific descriptions
        private static (string desc, string shortDesc) GetGenerationDescriptions(ITextFile entry, int generation)
        {
            string gen = $"gen{generation}";

            try
            {
                // Use reflection to get generation-specific properties
                var entryType = entry.GetType();
                var genProperty = entryType.GetProperty(gen);

                if (genProperty?.GetValue(entry) is IGenerationTextData genData)
                {
                    return (genData.Desc ?? "", genData.ShortDesc ?? "");
                }

                return ("", "");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting generation {generation} descriptions: {ex.Message}");
                return ("", "");
            }
        }

        // Helper method to get entry-level short description
        private static string? GetEntryShortDesc(ITextFile entry)
        {
            try
            {
                return entry switch
                {
                    AbilityText abilityText => abilityText.ShortDesc,
                    ItemText itemText => itemText.ShortDesc,
                    MoveText moveText => moveText.ShortDesc,
                    PokedexText pokedexText => pokedexText.ShortDesc,
                    DefaultText defaultText => defaultText.ShortDesc,
                    _ => null
                };
            }
            catch
            {
                return null;
            }
        }

        // Helper method to get entry-level description
        private static string? GetEntryDesc(ITextFile entry)
        {
            try
            {
                return entry switch
                {
                    AbilityText abilityText => abilityText.Desc,
                    ItemText itemText => itemText.Desc,
                    MoveText moveText => moveText.Desc,
                    PokedexText pokedexText => pokedexText.Desc,
                    DefaultText defaultText => defaultText.Desc,
                    _ => null
                };
            }
            catch
            {
                return null;
            }
        }

        public ActiveMove GetActiveMove(Move move)
        {
            throw new NotImplementedException("GetActiveMove method not implemented yet.");
        }

        public ActiveMove GetActiveMove(string name)
        {
            throw new NotImplementedException("GetActiveMove method not implemented yet.");
        }

        public HiddenPower GetHiddenPower(StatsTable ivs)
        {
            var hpTypes = new PokemonType[]
            {
                PokemonType.Fighting, PokemonType.Flying, PokemonType.Poison, PokemonType.Ground,
                PokemonType.Rock, PokemonType.Bug, PokemonType.Ghost, PokemonType.Steel,
                PokemonType.Fire, PokemonType.Water, PokemonType.Grass, PokemonType.Electric,
                PokemonType.Psychic, PokemonType.Ice, PokemonType.Dragon, PokemonType.Dark
            };

            // Helper method for truncation (Math.Floor for integer division)
            static int Trunc(double value) => (int)Math.Floor(value);

            if (Gen <= 2)
            {
                // Gen 2 specific Hidden Power check. IVs are still treated 0-31 so we convert to 0-15 DVs
                int atkDV = Trunc(ivs.Atk / 2.0);
                int defDV = Trunc(ivs.Def / 2.0);
                int speDV = Trunc(ivs.Spe / 2.0);
                int spcDV = Trunc(ivs.Spa / 2.0); // Special was combined in Gen 1-2

                return new HiddenPower
                {
                    Type = hpTypes[4 * (atkDV % 4) + (defDV % 4)],
                    Power = Trunc((5 * ((spcDV >> 3) + (2 * (speDV >> 3)) + (4 * (defDV >> 3)) + (8 * (atkDV >> 3))) + (spcDV % 4)) / 2.0 + 31)
                };
            }
            else
            {
                // Hidden Power check for Gen 3 onwards
                int hpTypeX = 0;
                int hpPowerX = 0;
                int i = 1;

                // Process stats in the correct order: HP, Atk, Def, Spe, SpA, SpD
                var statsInOrder = new int[] { ivs.Hp, ivs.Atk, ivs.Def, ivs.Spe, ivs.Spa, ivs.Spd };

                foreach (int statValue in statsInOrder)
                {
                    hpTypeX += i * (statValue % 2);
                    hpPowerX += i * (Trunc(statValue / 2.0) % 2);
                    i *= 2;
                }

                return new HiddenPower
                {
                    Type = hpTypes[Trunc(hpTypeX * 15.0 / 63)],
                    // After Gen 6, Hidden Power is always 60 base power
                    Power = (Gen > 0 && Gen < 6) ? Trunc(hpPowerX * 40.0 / 63) + 30 : 60
                };
            }
        }

        public List<AnyObject>? DataSearch(string target, List<DataSearchType>? searchIn = null,
            bool? isInexact = null)
        {
            throw new NotImplementedException("DataSearch method not implemented yet.");
        }

        public AnyObject? LoadDataFile(string basePath, DataType dataType)
        {
            try
            {
                // Get the file name for this data type
                if (!DataFiles.Files.TryGetValue(dataType, out string? fileName))
                {
                    throw new ArgumentException($"Unknown data type: {dataType}");
                }

                // Construct the file path
                string filePath = Path.Combine(basePath, $"{fileName}.json");

                // Check if file exists
                if (!File.Exists(filePath))
                {
                    return null;
                }

                // Load and validate the data
                var validatedData = LoadAndValidateDataFile(filePath, dataType);
                return validatedData;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        private static AnyObject? LoadAndValidateDataFile(string filePath, DataType dataType)
        {
            try
            {
                string jsonContent = File.ReadAllText(filePath);

                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    throw new InvalidDataException($"{filePath} must export a non-null object");
                }

                // Parse and validate JSON structure
                using var jsonDocument = JsonDocument.Parse(jsonContent);
                var rootElement = jsonDocument.RootElement;

                // Validate root is an object
                if (rootElement.ValueKind != JsonValueKind.Object)
                {
                    throw new InvalidDataException($"{filePath} must export a non-null object");
                }

                // Validate the specific data type property
                string dataTypeName = dataType.ToString();
                if (!rootElement.TryGetProperty(dataTypeName, out JsonElement dataElement))
                {
                    throw new InvalidDataException(
                        $"{filePath} must export an object whose '{dataTypeName}' property is an Object");
                }

                if (dataElement.ValueKind != JsonValueKind.Object)
                {
                    throw new InvalidDataException(
                        $"{filePath} must export an object whose '{dataTypeName}' property is an Object");
                }

                // Convert to strongly-typed data based on DataType
                return ConvertToTypedData(dataElement, dataType);
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException($"Invalid JSON in {filePath}: {ex.Message}", ex);
            }
        }

        private static AnyObject? ConvertToTypedData(JsonElement dataElement, DataType dataType)
        {
            try
            {
                return dataType switch
                {
                    DataType.Abilities => ConvertToAbilityData(dataElement),
                    DataType.Items => ConvertToItemData(dataElement),
                    DataType.Moves => ConvertToMoveData(dataElement),
                    DataType.Pokedex => ConvertToSpeciesData(dataElement),
                    DataType.FormatsData => ConvertToFormatsData(dataElement),
                    DataType.Natures => ConvertToNatureData(dataElement),
                    DataType.TypeChart => ConvertToTypeData(dataElement),
                    DataType.Learnsets => ConvertToLearnsetData(dataElement),
                    DataType.Conditions => ConvertToConditionData(dataElement),
                    DataType.Scripts => ConvertToScriptData(dataElement),
                    DataType.PokemonGoData => ConvertToPokemonGoData(dataElement),
                    DataType.Rulesets => ConvertToRulesetData(dataElement),
                    _ => ConvertToGenericData(dataElement)
                };
            }
            catch (Exception ex)
            {
                throw new InvalidDataException($"Failed to convert data for type {dataType}: {ex.Message}", ex);
            }
        }

        // Helper methods for type-specific conversion
        private static AnyObject ConvertToAbilityData(JsonElement element)
        {
            var abilities = JsonSerializer.Deserialize<Dictionary<string, AbilityData>>(element.GetRawText());
            return new AnyObject(abilities ?? new Dictionary<string, AbilityData>());
        }

        private static AnyObject ConvertToItemData(JsonElement element)
        {
            var items = JsonSerializer.Deserialize<Dictionary<string, ItemData>>(element.GetRawText());
            return new AnyObject(items ?? new Dictionary<string, ItemData>());
        }

        private static AnyObject ConvertToMoveData(JsonElement element)
        {
            var moves = JsonSerializer.Deserialize<Dictionary<string, MoveData>>(element.GetRawText());
            return new AnyObject(moves ?? new Dictionary<string, MoveData>());
        }

        private static AnyObject ConvertToSpeciesData(JsonElement element)
        {
            var species = JsonSerializer.Deserialize<Dictionary<string, SpeciesData>>(element.GetRawText());
            return new AnyObject(species ?? new Dictionary<string, SpeciesData>());
        }

        private static AnyObject ConvertToFormatsData(JsonElement element)
        {
            var formats = JsonSerializer.Deserialize<Dictionary<string, SpeciesFormatsData>>(element.GetRawText());
            return new AnyObject(formats ?? new Dictionary<string, SpeciesFormatsData>());
        }

        private static AnyObject ConvertToNatureData(JsonElement element)
        {
            var natures = JsonSerializer.Deserialize<Dictionary<string, NatureData>>(element.GetRawText());
            return new AnyObject(natures ?? new Dictionary<string, NatureData>());
        }

        private static AnyObject ConvertToTypeData(JsonElement element)
        {
            var types = JsonSerializer.Deserialize<Dictionary<string, TypeData>>(element.GetRawText());
            return new AnyObject(types ?? new Dictionary<string, TypeData>());
        }

        private static AnyObject ConvertToLearnsetData(JsonElement element)
        {
            var learnsets = JsonSerializer.Deserialize<Dictionary<string, LearnsetData>>(element.GetRawText());
            return new AnyObject(learnsets ?? new Dictionary<string, LearnsetData>());
        }

        private static AnyObject ConvertToConditionData(JsonElement element)
        {
            var conditions = JsonSerializer.Deserialize<Dictionary<string, IConditionData>>(element.GetRawText());
            return new AnyObject(conditions ?? new Dictionary<string, IConditionData>());
        }

        private static AnyObject ConvertToScriptData(JsonElement element)
        {
            var scripts = JsonSerializer.Deserialize<Dictionary<string, AnyObject>>(element.GetRawText());
            return new AnyObject(scripts ?? new Dictionary<string, AnyObject>());
        }

        private static AnyObject ConvertToPokemonGoData(JsonElement element)
        {
            var pokemonGoData = JsonSerializer.Deserialize<Dictionary<string, PokemonGoData>>(element.GetRawText());
            return new AnyObject(pokemonGoData ?? new Dictionary<string, PokemonGoData>());
        }

        private static AnyObject ConvertToRulesetData(JsonElement element)
        {
            var rulesets = JsonSerializer.Deserialize<Dictionary<string, FormatData>>(element.GetRawText());
            return new AnyObject(rulesets ?? new Dictionary<string, FormatData>());
        }

        private static AnyObject ConvertToGenericData(JsonElement element)
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(element.GetRawText());
            return new AnyObject(data ?? new Dictionary<string, object>());
        }

        public static DexTable<ITextFile> LoadTextFile(string name, string exportName)
        {
            try
            {
                // Construct the path to the text file
                string textFilePath = Path.Combine(DexConstants.DataDir, "text", $"{name}.json");

                // Check if file exists
                if (!File.Exists(textFilePath))
                {
                    Console.WriteLine($"Text file not found: {textFilePath}");
                    return new DexTable<ITextFile>();
                }

                // Read and deserialize the JSON file
                string jsonContent = File.ReadAllText(textFilePath);
                var jsonDocument = JsonDocument.Parse(jsonContent);

                // Extract the specified export
                if (!jsonDocument.RootElement.TryGetProperty(exportName, out JsonElement exportElement))
                {
                    Console.WriteLine($"Export '{exportName}' not found in {textFilePath}");
                    return new DexTable<ITextFile>();
                }

                // Deserialize based on the export name
                var result = new DexTable<ITextFile>();

                switch (exportName)
                {
                    case "PokedexText":
                        var pokedexData = JsonSerializer.Deserialize<Dictionary<string, PokedexText>>(exportElement.GetRawText());
                        if (pokedexData != null)
                        {
                            foreach (var (key, value) in pokedexData)
                            {
                                result[new Id(key)] = value;
                            }
                        }
                        break;

                    case "MovesText":
                        var movesData = JsonSerializer.Deserialize<Dictionary<string, MoveText>>(exportElement.GetRawText());
                        if (movesData != null)
                        {
                            foreach (var (key, value) in movesData)
                            {
                                result[new Id(key)] = value;
                            }
                        }
                        break;

                    case "AbilitiesText":
                        var abilitiesData = JsonSerializer.Deserialize<Dictionary<string, AbilityText>>(exportElement.GetRawText());
                        if (abilitiesData != null)
                        {
                            foreach (var (key, value) in abilitiesData)
                            {
                                result[new Id(key)] = value;
                            }
                        }
                        break;

                    case "ItemsText":
                        var itemsData = JsonSerializer.Deserialize<Dictionary<string, ItemText>>(exportElement.GetRawText());
                        if (itemsData != null)
                        {
                            foreach (var (key, value) in itemsData)
                            {
                                result[new Id(key)] = value;
                            }
                        }
                        break;

                    case "DefaultText":
                        var defaultData = JsonSerializer.Deserialize<Dictionary<string, DefaultText>>(exportElement.GetRawText());
                        if (defaultData != null)
                        {
                            foreach (var (key, value) in defaultData)
                            {
                                result[new Id(key)] = value;
                            }
                        }
                        break;

                    default:
                        Console.WriteLine($"Unknown export name: {exportName}");
                        break;
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading text file '{name}' with export '{exportName}': {ex.Message}");
                return new DexTable<ITextFile>();
            }
        }

        public ModdedDex IncludeMods()
        {
            // Ensure this is called only on the base dex
            if (!IsBase)
            {
                throw new InvalidOperationException("IncludeMods must be called on the base Dex");
            }

            // Return early if mods are already loaded
            if (ModsLoaded)
            {
                return this;
            }

            var loadedMods = new List<string>();

            try
            {
                string modsPath = Path.Combine(Environment.CurrentDirectory, DexConstants.ModsDir);

                if (!Directory.Exists(modsPath))
                {
                    Console.WriteLine($"Mods directory not found: {modsPath}. Creating directory.");
                    Directory.CreateDirectory(modsPath);
                    ModsLoaded = true;
                    return this;
                }

                // Get all subdirectories (potential mods)
                var modDirectories = Directory.GetDirectories(modsPath)
                    .Select(Path.GetFileName)
                    .Where(name => !string.IsNullOrEmpty(name) && IsValidModName(name!))
                    .ToArray();

                Console.WriteLine($"Found {modDirectories.Length} potential mods in {modsPath}");

                foreach (string modName in modDirectories)
                {
                    try
                    {
                        // Skip if mod is already registered
                        if (Dex.HasDex(modName))
                        {
                            Console.WriteLine($"Mod '{modName}' already registered, skipping.");
                            continue;
                        }

                        // Validate mod directory structure
                        string modPath = Path.Combine(modsPath, modName);
                        if (!IsValidModDirectory(modPath))
                        {
                            Console.WriteLine($"Invalid mod directory structure for '{modName}', skipping.");
                            continue;
                        }

                        // Create and register new ModdedDex for this mod
                        var modDex = new ModdedDex(modName);
                        Dex.RegisterDex(modName, modDex);
                        loadedMods.Add(modName);

                        Console.WriteLine($"Successfully loaded mod: {modName}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to load mod '{modName}': {ex.Message}");
                        // Continue with other mods
                    }
                }

                Console.WriteLine($"Loaded {loadedMods.Count} mods: {string.Join(", ", loadedMods)}");
                ModsLoaded = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical error in IncludeMods: {ex.Message}");
                // Still mark as loaded to prevent infinite retry attempts
                ModsLoaded = true;
            }

            return this;
        }

        public ModdedDex IncludeModData()
        {
            var processedMods = new List<string>();
            var failedMods = new List<string>();

            try
            {
                var allDexes = Dex.GetAllDexes();
                Console.WriteLine($"Loading data for {allDexes.Count} registered dexes...");

                foreach (var (modName, modDex) in allDexes)
                {
                    try
                    {
                        Console.WriteLine($"Loading data for mod: {modName}");
                        modDex.IncludeData();
                        processedMods.Add(modName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to load data for mod '{modName}': {ex.Message}");
                        failedMods.Add(modName);
                    }
                }

                Console.WriteLine($"Successfully loaded data for {processedMods.Count} mods");
                if (failedMods.Count > 0)
                {
                    Console.WriteLine($"Failed to load data for {failedMods.Count} mods: {string.Join(", ", failedMods)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical error in IncludeModData: {ex.Message}");
            }

            return this;
        }

        // Helper methods
        private static bool IsValidModName(string modName)
        {
            // Validate mod name format
            if (string.IsNullOrWhiteSpace(modName))
                return false;

            // Check for invalid characters
            char[] invalidChars = Path.GetInvalidFileNameChars();
            if (modName.Any(c => invalidChars.Contains(c)))
                return false;

            // Additional validation rules
            if (modName.StartsWith('.') || modName.Length > 50)
                return false;

            return true;
        }

        private static bool IsValidModDirectory(string modPath)
        {
            try
            {
                // Check if directory exists and is accessible
                if (!Directory.Exists(modPath))
                    return false;

                // Optional: Check for required mod files
                // string modJsonPath = Path.Combine(modPath, "mod.json");
                // if (!File.Exists(modJsonPath))
                //     return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public ModdedDex IncludeData()
        {
            LoadData();
            return this;
        }

        public TextTableData LoadTextData()
        {
            try
            {
                // Check if base dex already has cached text data
                ModdedDex baseDex = Dex.BaseDex;
                if (baseDex.TextCache != null)
                {
                    return baseDex.TextCache;
                }

                // Load text data for each category
                var textData = new TextTableData();

                // Load each text file with error handling
                try
                {
                    textData.Pokedex = CastTextTable<PokedexText>(LoadTextFile("pokedex", "PokedexText"));
                }
                catch (Exception ex)
                {
                    // Log error and use empty table
                    Console.WriteLine($"Failed to load Pokedex text: {ex.Message}");
                    textData.Pokedex = new DexTable<PokedexText>();
                }

                try
                {
                    textData.Moves = CastTextTable<MoveText>(LoadTextFile("moves", "MovesText"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load Moves text: {ex.Message}");
                    textData.Moves = new DexTable<MoveText>();
                }

                try
                {
                    textData.Abilities = CastTextTable<AbilityText>(LoadTextFile("abilities", "AbilitiesText"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load Abilities text: {ex.Message}");
                    textData.Abilities = new DexTable<AbilityText>();
                }

                try
                {
                    textData.Items = CastTextTable<ItemText>(LoadTextFile("items", "ItemsText"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load Items text: {ex.Message}");
                    textData.Items = new DexTable<ItemText>();
                }

                try
                {
                    textData.Default = CastTextTable<DefaultText>(LoadTextFile("default", "DefaultText"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load Default text: {ex.Message}");
                    textData.Default = new DexTable<DefaultText>();
                }

                // Cache the loaded text data in the base dex
                baseDex.TextCache = textData;

                return textData;
            }
            catch (Exception ex)
            {
                // Return empty text data if everything fails
                Console.WriteLine($"Failed to load text data: {ex.Message}");
                return new TextTableData();
            }
        }

        // Helper method to safely cast text tables
        private static DexTable<T> CastTextTable<T>(DexTable<ITextFile> source) where T : class, ITextFile
        {
            var result = new DexTable<T>();

            foreach ((Id key, var value) in source)
            {
                if (value is T typedValue)
                {
                    result[key] = typedValue;
                }
                else
                {
                    // Log warning or throw exception for type mismatch
                    throw new InvalidCastException($"Cannot cast {value?.GetType().Name} to {typeof(T).Name}");
                }
            }

            return result;
        }

        // Updated GetAlias method
        public Id? GetAlias(Id id)
        {
            LoadAliases(); // Ensure aliases are loaded
            return Aliases?.GetValueOrDefault(id);
        }

        public ModdedDex LoadAliases()
        {
            // Delegate to base dex if this is not the base mod
            if (!IsBase)
                return Dex.BaseDex.LoadAliases();

            // Return cached aliases if already loaded
            if (Aliases is { Count: > 0 })
                return this;

            // Load aliases data from file
            AliasesFileData aliasesData = LoadAliasesDataFromFile();
            var aliases = new Dictionary<Id, Id>();
            var compoundNames = new Dictionary<Id, string>();
            var fuzzyAliases = new Dictionary<Id, List<Id>>();

            // Load direct aliases
            foreach ((string alias, string target) in aliasesData.Aliases)
            {
                aliases[new Id(alias)] = new Id(target);
            }

            // Load compound word names
            foreach (string name in aliasesData.CompoundWordNames)
            {
                compoundNames[new Id(name)] = name;
            }

            // Helper function to add fuzzy aliases
            void AddFuzzy(Id alias, Id target)
            {
                if (alias == target) return;
                if (alias.Value.Length < 2) return;

                if (!fuzzyAliases.TryGetValue(alias, out var prev))
                {
                    prev = [];
                    fuzzyAliases[alias] = prev;
                }

                if (!prev.Contains(target))
                {
                    prev.Add(target);
                }
            }

            // Helper function to add fuzzy forme aliases
            void AddFuzzyForme(Id alias, Id target, Id forme, Id formeLetter)
            {
                AddFuzzy(new Id($"{alias}{forme}"), target);

                if (forme.IsEmpty) return;

                AddFuzzy(new Id($"{alias}{formeLetter}"), target);
                AddFuzzy(new Id($"{formeLetter}{alias}"), target);

                // Regional forme aliases
                switch (forme.Value)
                {
                    case "alola":
                        AddFuzzy(new Id($"alolan{alias}"), target);
                        break;
                    case "galar":
                        AddFuzzy(new Id($"galarian{alias}"), target);
                        break;
                    case "hisui":
                        AddFuzzy(new Id($"hisuian{alias}"), target);
                        break;
                    case "paldea":
                        AddFuzzy(new Id($"paldean{alias}"), target);
                        break;
                    case "megax":
                        AddFuzzy(new Id($"mega{alias}x"), target);
                        break;
                    case "megay":
                        AddFuzzy(new Id($"mega{alias}y"), target);
                        break;
                    default:
                        AddFuzzy(new Id($"{forme}{alias}"), target);
                        break;
                }

                // Mega forme special aliases
                if (forme.Value is not ("megax" or "megay")) return;
                AddFuzzy(new Id($"mega{alias}"), target);
                AddFuzzy(new Id($"{alias}mega"), target);
                AddFuzzy(new Id($"m{alias}"), target);
                AddFuzzy(new Id($"{alias}m"), target);
            }

            // Process each data table
            string[] tables = ["Items", "Abilities", "Moves", "Pokedex"];

            foreach (string table in tables)
            {
                var tableData = GetTableData(table);

                foreach ((Id id, object entry) in tableData)
                {
                    string name = compoundNames.GetValueOrDefault(id) ?? GetEntryName(entry);
                    var forme = Id.Empty;
                    var formeLetter = Id.Empty;

                    // Handle parenthetical names (e.g., "Pokemon (Form)")
                    if (name.Contains('('))
                    {
                        AddFuzzy(new Id(name.Split('(')[0].Trim()), id);
                    }

                    // Special handling for Pokedex entries
                    if (table == "Pokedex")
                    {
                        if (entry is SpeciesData species)
                        {
                            var baseId = new Id(species.BaseSpecies);
                            if (!baseId.IsEmpty && baseId != id)
                            {
                                name = compoundNames.GetValueOrDefault(baseId) ?? baseId.Value;
                            }

                            forme = new Id(species.Forme ?? species.BaseForme ?? "");

                            // Special forme letter mappings
                            formeLetter = forme.Value switch
                            {
                                "fan" => new Id("s"),
                                "bloodmoon" => new Id("bm"),
                                _ => CreateFormeLetterFromName(species.Forme ?? "")
                            };

                            AddFuzzy(forme, id);
                        }
                    }

                    // Add fuzzy aliases for the main name
                    AddFuzzyForme(new Id(name), id, forme, formeLetter);

                    // Process name splits (space and hyphen)
                    string[] fullSplit = SplitAndConvertToIds(name, [' ', '-']);
                    if (fullSplit.Length >= 2)
                    {
                        // Create acronym from first letters
                        string fullAcronym = string.Join("", fullSplit.Select(part =>
                            part.Length > 0 ? part[0].ToString() : ""));
                        AddFuzzyForme(new Id(fullAcronym), id, forme, formeLetter);

                        // Create acronym + last word
                        if (fullSplit.Length > 0)
                        {
                            string lastWord = fullSplit[^1];
                            if (lastWord.Length > 1)
                            {
                                string fullAcronymWord = fullAcronym + lastWord[1..];
                                AddFuzzyForme(new Id(fullAcronymWord), id, forme, formeLetter);
                            }
                        }

                        // Add each word part
                        foreach (string wordPart in fullSplit)
                        {
                            AddFuzzyForme(new Id(wordPart), id, forme, formeLetter);
                        }
                    }

                    // Process space-only splits if different from full split
                    string[] spaceSplit = SplitAndConvertToIds(name, [' ']);
                    if (spaceSplit.Length == fullSplit.Length) continue;
                    {
                        string spaceAcronym = string.Join("", spaceSplit.Select(part =>
                            part.Length > 0 ? part[0].ToString() : ""));
                        AddFuzzyForme(new Id(spaceAcronym), id, forme, formeLetter);

                        if (spaceSplit.Length > 0)
                        {
                            string lastWord = spaceSplit[^1];
                            if (lastWord.Length > 1)
                            {
                                string spaceAcronymWord = spaceAcronym + lastWord[1..];
                                AddFuzzyForme(new Id(spaceAcronymWord), id, forme, formeLetter);
                            }
                        }

                        foreach (string word in spaceSplit)
                        {
                            AddFuzzyForme(new Id(word), id, forme, formeLetter);
                        }
                    }
                }
            }

            // Store the loaded aliases (using mutable assignment since these are init-only)
            Aliases = aliases;
            FuzzyAliases = fuzzyAliases;

            return this;
        }

        private static AliasesFileData LoadAliasesDataFromFile()
        {
            // Load from data/aliases.json or similar
            // This would be implemented based on your file loading system
            string aliasesPath = Path.Combine(DexConstants.DataDir, "aliases.json");

            if (!File.Exists(aliasesPath))
            {
                return new AliasesFileData
                {
                    Aliases = new Dictionary<string, string>(),
                    CompoundWordNames = []
                };
            }

            string jsonContent = File.ReadAllText(aliasesPath);
            return JsonSerializer.Deserialize<AliasesFileData>(jsonContent) ??
                new AliasesFileData
                {
                    Aliases = new Dictionary<string, string>(),
                    CompoundWordNames = []
                };
        }

        private Dictionary<Id, object> GetTableData(string tableName)
        {
            return tableName switch
            {
                "Items" => Data.Items.ToDictionary(kv => kv.Key, object (kv) => kv.Value),
                "Abilities" => Data.Abilities.ToDictionary(kv => kv.Key, object (kv) => kv.Value),
                "Moves" => Data.Moves.ToDictionary(kv => kv.Key, object (kv) => kv.Value),
                "Pokedex" => Data.Pokedex.ToDictionary(kv => kv.Key, object (kv) => kv.Value),
                _ => new Dictionary<Id, object>()
            };
        }

        private static string GetEntryName(object entry)
        {
            // Use reflection or pattern matching to get the Name property
            return entry switch
            {
                SpeciesData species => species.Name,
                ItemData item => item.Name,
                AbilityData ability => ability.Name,
                MoveData move => move.Name,
                _ => ""
            };
        }

        private static Id CreateFormeLetterFromName(string formeName)
        {
            if (string.IsNullOrEmpty(formeName)) return Id.Empty;

            // Split by space or hyphen and take first letter of each part
            string[] parts = formeName.Split([' ', '-'], StringSplitOptions.RemoveEmptyEntries);
            string letters = string.Join("", parts.Select(part =>
                part.Length > 0 ? new Id(part).Value.Take(1).First().ToString() : ""));

            return new Id(letters);
        }

        private static string[] SplitAndConvertToIds(string name, char[] separators)
        {
            return name.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                       .Select(part => new Id(part).Value)
                       .Where(id => !string.IsNullOrEmpty(id))
                       .ToArray();
        }

        // Data structure for loading aliases file
        private class AliasesFileData
        {
            public Dictionary<string, string> Aliases { get; set; } = new();
            public List<string> CompoundWordNames { get; set; } = [];
        }

        public DexTableData LoadData()
        {
            // Return cached data if available
            if (DataCache != null)
                return DataCache;

            // Ensure mods are loaded
            Dex.BaseDex.IncludeMods();

            // Initialize data cache
            var dataCache = new Dictionary<DataType, object>();

            // Construct base path
            string basePath = DataDir + "/";

            // Load scripts to determine inheritance
            var scriptsData = LoadDataFile(basePath, DataType.Scripts);
            var scripts = scriptsData ?? new AnyObject() { ["gen"] = 9 };

            // Extract initialization function and parent mod
            var initFunction = GetInitFunction(scripts);
            string parentModName = IsBase ? "" : GetParentMod(scripts);

            ModdedDex? parentDex = null;
            if (!string.IsNullOrEmpty(parentModName))
            {
                parentDex = ValidateParentMod(parentModName);
            }

            // Include formats if no parent (base mod)
            if (parentDex == null)
            {
                IncludeFormats();
            }

            // Load all data types
            LoadAllDataTypes(basePath, dataCache, parentDex);

            // Process inheritance from parent mod
            if (parentDex != null)
            {
                ProcessInheritance(dataCache, parentDex, initFunction != null);
            }

            // Convert to strongly-typed data cache
            var typedDataCache = ConvertToTypedDataCache(dataCache);

            // Set generation from scripts
            SetGenerationFromScripts(scripts);

            // Cache the result
            DataCache = typedDataCache;

            // Execute initialization script
            ExecuteInitFunction(initFunction);

            return DataCache;
        }

        // Helper method to get initialization function from scripts
        private static Delegate? GetInitFunction(AnyObject scripts)
        {
            try
            {
                // Try to extract init function from scripts
                // This would depend on how scripts are structured
                if (scripts.TryGetValue("init", out object? initValue) && initValue is Delegate initFunction)
                {
                    return initFunction;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting init function: {ex.Message}");
                return null;
            }
        }

        // Helper method to get parent mod name
        private static string GetParentMod(AnyObject scripts)
        {
            try
            {
                if (scripts.TryGetValue("inherit", out object? inheritValue) && inheritValue is string inherit)
                {
                    return inherit;
                }
                return "base"; // Default parent
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting parent mod: {ex.Message}");
                return "base";
            }
        }

        // Helper method to validate parent mod
        private ModdedDex ValidateParentMod(string parentModName)
        {
            var parentDex = Dex.GetDex(parentModName);

            if (parentDex == null || parentDex == this)
            {
                throw new InvalidOperationException(
                    $"Unable to load {CurrentMod}. 'inherit' in scripts should specify a valid parent mod, or must not be specified.");
            }

            return parentDex;
        }

        // Helper method to load all data types
        private void LoadAllDataTypes(string basePath, Dictionary<DataType, object> dataCache, ModdedDex? parentDex)
        {
            var dataTypes = Enum.GetValues<DataType>();

            foreach (DataType dataType in dataTypes)
            {
                try
                {
                    var data = LoadDataFile(basePath, dataType);
                    dataCache[dataType] = data ?? CreateEmptyDataTable(dataType);

                    // Special handling for Rulesets when no parent
                    if (dataType == DataType.Rulesets && parentDex == null)
                    {
                        AddFormatsToRulesets(dataCache);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading {dataType}: {ex.Message}");
                    dataCache[dataType] = CreateEmptyDataTable(dataType);
                }
            }
        }

        // Helper method to add formats to rulesets
        private void AddFormatsToRulesets(Dictionary<DataType, object> dataCache)
        {
            try
            {
                if (dataCache[DataType.Rulesets] is Dictionary<string, object> rulesets)
                {
                    var allFormats = Formats.All();
                    foreach (var format in allFormats)
                    {
                        rulesets[format.Id.Value] = new
                        {
                            format.Name,
                            format.Id,
                            format.Gen,
                            format.Exists,
                            // Copy other relevant format properties
                            ruleTable = (object?)null // Set ruleTable to null as in original
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding formats to rulesets: {ex.Message}");
            }
        }

        // Helper method to process inheritance
        private static void ProcessInheritance(Dictionary<DataType, object> dataCache, ModdedDex parentDex, bool hasInitFunction)
        {
            var dataTypes = Enum.GetValues<DataType>();

            foreach (DataType dataType in dataTypes)
            {
                try
                {
                    ProcessDataTypeInheritance(dataCache, parentDex, dataType, hasInitFunction);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing inheritance for {dataType}: {ex.Message}");
                }
            }
        }

        // Helper method to process inheritance for a specific data type
        private static void ProcessDataTypeInheritance(Dictionary<DataType, object> dataCache, ModdedDex parentDex,
            DataType dataType, bool hasInitFunction)
        {
            var parentTypedData = GetParentDataTable(parentDex, dataType);

            if (!dataCache.ContainsKey(dataType) && !hasInitFunction)
            {
                // No child data and no init function - inherit parent data directly
                dataCache[dataType] = parentTypedData;
                return;
            }

            // Get or create child data table
            if (!dataCache.TryGetValue(dataType, out object? childDataObj))
            {
                childDataObj = CreateEmptyDataTable(dataType);
                dataCache[dataType] = childDataObj;
            }

            if (childDataObj is not Dictionary<string, object> childTypedData)
                return;

            if (parentTypedData is not Dictionary<string, object> parentData)
                return;

            // Process each entry in parent data
            foreach (var (entryId, parentEntry) in parentData)
            {
                ProcessEntryInheritance(childTypedData, entryId, parentEntry);
            }
        }

        // Helper method to process inheritance for a single entry
        private static void ProcessEntryInheritance(Dictionary<string, object> childTypedData, string entryId, object parentEntry)
        {
            if (childTypedData.TryGetValue(entryId, out object? childEntry))
            {
                if (childEntry == null)
                {
                    // null means don't inherit - remove the entry
                    childTypedData.Remove(entryId);
                }
                else if (HasInheritFlag(childEntry))
                {
                    // {inherit: true} - merge parent and child data
                    var mergedEntry = MergeEntries(parentEntry, childEntry);
                    RemoveInheritFlag(mergedEntry);
                    childTypedData[entryId] = mergedEntry;
                }
                // Otherwise, child entry completely overrides parent
            }
            else
            {
                // Entry doesn't exist in child - inherit from parent
                childTypedData[entryId] = parentEntry;
            }
        }

        // Helper method to check if entry has inherit flag
        private static bool HasInheritFlag(object entry)
        {
            try
            {
                if (entry is Dictionary<string, object> dict)
                {
                    return dict.ContainsKey("inherit") && dict["inherit"] is true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Helper method to merge parent and child entries
        private static object MergeEntries(object parentEntry, object childEntry)
        {
            try
            {
                if (parentEntry is Dictionary<string, object> parentDict &&
                    childEntry is Dictionary<string, object> childDict)
                {
                    var merged = new Dictionary<string, object>(parentDict);

                    // Child properties override parent properties
                    foreach (var (key, value) in childDict)
                    {
                        merged[key] = value;
                    }

                    return merged;
                }

                // If not dictionaries, child overrides parent
                return childEntry;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error merging entries: {ex.Message}");
                return childEntry;
            }
        }

        // Helper method to remove inherit flag
        private static void RemoveInheritFlag(object entry)
        {
            if (entry is Dictionary<string, object> dict)
            {
                dict.Remove("inherit");
            }
        }

        // Helper method to get parent data table
        private static object GetParentDataTable(ModdedDex parentDex, DataType dataType)
        {
            return dataType switch
            {
                DataType.Abilities => parentDex.Data.Abilities,
                DataType.Items => parentDex.Data.Items,
                DataType.Moves => parentDex.Data.Moves,
                DataType.Pokedex => parentDex.Data.Pokedex,
                DataType.FormatsData => parentDex.Data.FormatsData,
                DataType.Natures => parentDex.Data.Natures,
                DataType.TypeChart => parentDex.Data.TypeChart,
                DataType.Learnsets => parentDex.Data.Learnsets,
                DataType.Conditions => parentDex.Data.Conditions,
                DataType.Scripts => parentDex.Data.Scripts,
                DataType.PokemonGoData => parentDex.Data.PokemonGoData,
                DataType.Rulesets => parentDex.Data.Rulesets,
                _ => new Dictionary<string, object>()
            };
        }

        // Helper method to create empty data table
        private static object CreateEmptyDataTable(DataType dataType)
        {
            return dataType switch
            {
                DataType.Abilities => new DexTable<AbilityData>(),
                DataType.Items => new DexTable<ItemData>(),
                DataType.Moves => new DexTable<MoveData>(),
                DataType.Pokedex => new DexTable<SpeciesData>(),
                DataType.FormatsData => new DexTable<SpeciesFormatsData>(),
                DataType.Natures => new DexTable<NatureData>(),
                DataType.TypeChart => new DexTable<TypeData>(),
                DataType.Learnsets => new DexTable<LearnsetData>(),
                DataType.Conditions => new DexTable<IConditionData>(),
                DataType.Scripts => new DexTable<AnyObject>(),
                DataType.PokemonGoData => new DexTable<PokemonGoData>(),
                DataType.Rulesets => new DexTable<FormatData>(),
                _ => new Dictionary<string, object>()
            };
        }

        // Helper method to convert to typed data cache
        private DexTableData ConvertToTypedDataCache(Dictionary<DataType, object> dataCache)
        {
            return new DexTableData
            {
                Abilities = ConvertToTypedTable<AbilityData>(dataCache, DataType.Abilities),
                Items = ConvertToTypedTable<ItemData>(dataCache, DataType.Items),
                Moves = ConvertToTypedTable<MoveData>(dataCache, DataType.Moves),
                Pokedex = ConvertToTypedTable<SpeciesData>(dataCache, DataType.Pokedex),
                FormatsData = ConvertToTypedTable<SpeciesFormatsData>(dataCache, DataType.FormatsData),
                Natures = ConvertToTypedTable<NatureData>(dataCache, DataType.Natures),
                TypeChart = ConvertToTypedTable<TypeData>(dataCache, DataType.TypeChart),
                Learnsets = ConvertToTypedTable<LearnsetData>(dataCache, DataType.Learnsets),
                Conditions = ConvertToTypedTable<IConditionData>(dataCache, DataType.Conditions),
                Scripts = ConvertToTypedTable<AnyObject>(dataCache, DataType.Scripts),
                PokemonGoData = ConvertToTypedTable<PokemonGoData>(dataCache, DataType.PokemonGoData),
                Rulesets = ConvertToTypedTable<FormatData>(dataCache, DataType.Rulesets)
            };
        }

        // Helper method to convert to typed table
        private static DexTable<T> ConvertToTypedTable<T>(Dictionary<DataType, object> dataCache, DataType dataType)
        {
            try
            {
                if (dataCache.TryGetValue(dataType, out object? data) && data is DexTable<T> typedTable)
                {
                    return typedTable;
                }

                return new DexTable<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting {dataType} to typed table: {ex.Message}");
                return new DexTable<T>();
            }
        }

        // Helper method to set generation from scripts
        private void SetGenerationFromScripts(AnyObject scripts)
        {
            try
            {
                if (scripts.TryGetValue("gen", out object? genValue) && genValue is int generation)
                {
                    Gen = generation;
                }
                else
                {
                    throw new InvalidOperationException($"Mod {CurrentMod} needs a generation number in scripts");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Mod {CurrentMod} needs a generation number in scripts: {ex.Message}");
            }
        }

        // Helper method to execute initialization function
        private void ExecuteInitFunction(Delegate? initFunction)
        {
            try
            {
                if (initFunction != null)
                {
                    // Execute init function with this ModdedDex as context
                    initFunction.DynamicInvoke(this);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing init function for {CurrentMod}: {ex.Message}");
            }
        }

        private ModdedDex IncludeFormats()
        {
            Formats.Load();
            return this;
        }
    }
}