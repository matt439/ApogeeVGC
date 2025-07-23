using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC_CS.sim
{
    // Data type enum
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

    // Generic DexTable
    public class DexTable<T> : Dictionary<string, T> { }

    // Aliases table
    public class AliasesTable : Dictionary<string, string> { }

    // DexTableData structure
    public class DexTableData
    {
        public DexTable<AbilityData> Abilities { get; set; } = new();
        public DexTable<FormatData> Rulesets { get; set; } = new();
        public DexTable<ItemData> Items { get; set; } = new();
        public DexTable<LearnsetData> Learnsets { get; set; } = new();
        public DexTable<MoveData> Moves { get; set; } = new();
        public DexTable<NatureData> Natures { get; set; } = new();
        public DexTable<SpeciesData> Pokedex { get; set; } = new();
        public DexTable<SpeciesFormatsData> FormatsData { get; set; } = new();
        public DexTable<PokemonGoData> PokemonGoData { get; set; } = new();
        public DexTable<object> Scripts { get; set; } = new();
        public DexTable<ConditionData> Conditions { get; set; } = new();
        public DexTable<TypeData> TypeChart { get; set; } = new();
    }

    // TextTableData structure
    public class TextTableData
    {
        public DexTable<AbilityText> Abilities { get; set; } = new();
        public DexTable<ItemText> Items { get; set; } = new();
        public DexTable<MoveText> Moves { get; set; } = new();
        public DexTable<PokedexText> Pokedex { get; set; } = new();
        public DexTable<DefaultText> Default { get; set; } = new();
    }

    public class ModdedDex
    {
        public string Name { get; } = "[ModdedDex]";
        public bool IsBase { get; }
        public string CurrentMod { get; }
        public string DataDir { get; }
        public int Gen { get; set; } = 0;
        public string ParentMod { get; set; } = string.Empty;
        public bool ModsLoaded { get; set; } = false;

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

        public Dictionary<string, string>? Aliases { get; set; }
        public Dictionary<string, List<string>>? FuzzyAliases { get; set; }

        public ModdedDex(string mod = "base")
        {
            IsBase = (mod == "base");
            CurrentMod = mod;
            DataDir = IsBase ? "DATA_DIR" : $"MODS_DIR/{CurrentMod}";

            DataCache = null;
            TextCache = null;

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
                return new Dictionary<string, ModdedDex>(); // Replace with actual dexes
            }
        }

        private DexTableData LoadData()
        {
            // TODO: Implement data loading logic
            return new DexTableData();
        }

        private void IncludeMods()
        {
            // TODO: Implement mod inclusion logic
        }

        // TODO: Implement other methods as needed
    }

    public class Dex
    {
        public Dictionary<string, ModdedDex> Dexes = new();

        Dex()
        {
            // Initialize base dex
            var baseDex = new ModdedDex("base");
            Dexes[baseDex.Name] = baseDex;
        }
    }
}