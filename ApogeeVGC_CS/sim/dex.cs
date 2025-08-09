namespace ApogeeVGC_CS.sim
{
    public class Dex
    {
        public Dictionary<string, ModdedDex> Dexes { get; init; } = [];

        public Dex()
        {
            // Initialize the base modded dex
            var baseDex = new ModdedDex();
            Dexes[baseDex.CurrentMod] = baseDex;
            Dexes[DexConstants.BaseMod.ToString()] = baseDex;
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
        public DexTable<IAnyObject> Scripts { get; init; } = [];
        public DexTable<IConditionData> Conditions { get; init; } = [];
        public DexTable<TypeData> TypeChart { get; init; } = [];
    }

    public class TextTableData
    {
        public DexTable<AbilityText> Abilities { get; init; } = [];
        public DexTable<ItemText> Items { get; init; } = [];
        public DexTable<MoveText> Moves { get; init; } = [];
        public DexTable<PokedexText> Pokedex { get; init; } = [];
        public DexTable<DefaultText> Default { get; init; } = [];
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

    // MoveText | ItemText | AbilityText | PokedexText | DefaultText
    public interface ITextFile;

    public class ModdedDex
    {
        public static string Name => "[ModdedDex]";
        public bool IsBase => CurrentMod == "base";
        public string CurrentMod { get; init; }
        public string DataDir => IsBase ? DexConstants.DataDir : $"{DexConstants.ModsDir}/{CurrentMod}";
        public int Gen { get; init; } = 0;
        public string ParentMod { get; init; } = string.Empty;
        public bool ModsLoaded { get; init; } = false;

        public DexTableData? DataCache { get; init; } = null;
        public TextTableData? TextCache { get; init; } = null;

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

        public Dictionary<Id, Id>? Aliases { get; init; } = null;
        public Dictionary<Id, List<Id>>? FuzzyAliases { get; init; } = null;

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

        public Dictionary<string, ModdedDex> Dexes
        {
            get
            {
                IncludeMods();
                throw new NotImplementedException();
            }
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

        public Descriptions GetDescs(TextTableData table, Id id, object dataEntry)
        {
            throw new NotImplementedException("GetDescs method not implemented yet.");
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
            throw new NotImplementedException("GetHiddenPower method not implemented yet.");
        }

        // int Trunc()

        public List<object>? DataSearch(string target, List<DataSearchType>? searchIn = null,
            bool? isInexact = null)
        {
            throw new NotImplementedException("DataSearch method not implemented yet.");
        }

        public object? LoadDataFile(string basePath, DataType dataType)
        {
            throw new NotImplementedException("LoadDataFile method not implemented yet.");
        }

        public DexTable<ITextFile> LoadTextFile(string name, string exportName)
        {
            throw new NotImplementedException("LoadTextFile method not implemented yet.");
        }
        public ModdedDex IncludeMods()
        {
            throw new NotImplementedException();
        }

        public ModdedDex IncludeModData()
        {
            throw new NotImplementedException();
        }

        public ModdedDex IncludeData()
        {
            throw new NotImplementedException();
        }

        public TextTableData LoadTextData()
        {
            throw new NotImplementedException("LoadTextData method not implemented yet.");
        }

        public Id? GetAlias(Id id)
        {
            throw new NotImplementedException("GetAlias method not implemented yet.");
        }

        public ModdedDex LoadAliases()
        {
            throw new NotImplementedException("LoadAliases method not implemented yet.");
        }

        public DexTableData LoadData()
        {
            throw new NotImplementedException("LoadAliases method not implemented yet.");
        }
    }
}