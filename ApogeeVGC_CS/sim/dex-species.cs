using ApogeeVGC_CS.config;
using ApogeeVGC_CS.data;
using ApogeeVGC_CS.sim;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC_CS.sim
{
    public class SpeciesAbility
    {
        public required string Slot0 { get; init; }
        public string? Slot1 { get; init; }
        public string? Hidden { get; init; }
        public string? Special { get; init; }
    }

    public enum SpeciesTag
    {
        Mythical,
        RestrictedLegendary,
        SubLegendary,
        UltraBeast,
        Paradox,
    }

    public class SpeciesData : Species;

    //public class SpeciesData(ISpeciesData other) : ISpeciesData
    //{
    //    public string Name { get; set; } = other.Name;
    //    public int Num { get; set; } = other.Num;
    //    public List<PokemonType> Types { get; set; } = [.. other.Types];
    //    public SpeciesAbility Abilities { get; set; } = new(other.Abilities);
    //    public StatsTable BaseStats { get; set; } = new(other.BaseStats);
    //    public List<string> EggGroups { get; set; } = [.. other.EggGroups];
    //    public double WeightKg { get; set; } = other.WeightKg;
    //}

    public class ModdedSpeciesData : SpeciesData
    {
        public static bool Inherit => true;
    }

    public interface ISpeciesFormatsData
    {
        Tier? DoublesTier { get; }
        bool? GmaxUnreleased { get; }
        Nonstandard? IsNonstandard { get; }
        Tier? NatDexTier { get; }
        Tier? Tier { get; }
    }

    public class SpeciesFormatsData : ISpeciesFormatsData
    {
        public Tier? DoublesTier
        {
            get;
            init // TierTypes.Doubles | TierTypes.Other
            {
                if (value == null || TierTools.IsDoublesOrOtherTier(value))
                {
                    field = value;
                }
            }
        }
        public bool? GmaxUnreleased { get; init; }
        public Nonstandard? IsNonstandard { get; init; }
        public Tier? NatDexTier
        {
            get;
            init // TierTypes.Singles | TierTypes.Other
            {
                if (value == null || TierTools.IsSinglesOrOtherTier(value))
                {
                    field = value;
                }
            }
        }
        
        public Tier? Tier
        {
            get;
            init // TierTypes.Singles | TierTypes.Other
            {
                if (value == null || TierTools.IsSinglesOrOtherTier(value))
                {
                    field = value;
                }
            }
        }
    }

    public class ModdedSpeciesFormatsData : SpeciesFormatsData
    {
        public static bool? Inherit => true;
    }

    public class LearnsetData
    {
        public Dictionary<IdEntry, List<MoveSource>>? Learnset { get; init; }
        public List<EventInfo>? EventData { get; init; }
        public bool? EventOnly { get; init; }
        public List<EventInfo>? Encounters { get; init; }
        public bool? Exists { get; init; }
    }

    public class ModdedLearnsetData : LearnsetData
    {
        public static bool Inherit => true;
    }

    public class PokemonGoData
    {
        public List<string>? Encounters { get; init; }
        public Dictionary<string, int?>? LgpeRestrictiveMoves { get; init; }
    }

    public class SpeciesDataTable : Dictionary<IdEntry, SpeciesData>;

    public class ModdedSpeciesDataTable : Dictionary<IdEntry, ModdedSpeciesData>;

    public class SpeciesFormatsDataTable : Dictionary<IdEntry, SpeciesFormatsData>;

    public class ModdedSpeciesFormatsDataTable : Dictionary<IdEntry, ModdedSpeciesFormatsData>;

    public class LearnsetDataTable : Dictionary<IdEntry, LearnsetData>;

    public class ModdedLearnsetDataTable : Dictionary<IdEntry, ModdedLearnsetData>;

    public class PokemonGoDataTable : Dictionary<IdEntry, PokemonGoData>;

    /// <summary>
    /// Describes a possible way to get a move onto a Pokémon.
    /// <para>
    /// Format: The first character is a generation number (1-9). The second character is a source ID:
    /// <list type="bullet">
    /// <item><description>M = TM / HM</description></item>
    /// <item><description>T = tutor</description></item>
    /// <item><description>L = start or level-up, 3rd char+ is the level</description></item>
    /// <item><description>R = restricted (special moves like Rotom moves)</description></item>
    /// <item><description>E = egg</description></item>
    /// <item><description>D = Dream World, only 5D is valid</description></item>
    /// <item><description>S = event, 3rd char+ is the index in .eventData</description></item>
    /// <item><description>V = Virtual Console or Let's Go transfer, only 7V/8V is valid</description></item>
    /// <item><description>C = NOT A REAL SOURCE, see note, only 3C/4C is valid</description></item>
    /// </list>
    /// C marks certain moves learned by a Pokémon's pre-evolution. It's used to work around the chainbreeding checker's shortcuts for performance;
    /// it lets the Pokémon be a valid father for teaching the move, but is otherwise ignored by the learnset checker (which will actually check prevos for compatibility).
    /// </para>
    /// </summary>

    public enum MoveSourceType
    {
        Tm, Tutor, LevelUp, Restricted, Egg, DreamWorld, Event, Virtual, Chain
    }

    public class MoveSource
    {
        public int Generation
        {
            get;
            init
            {
                if (value is < 1 or > 9)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Generation must be between 1 and 9.");
                }
                field = value;
            }
        }
        public MoveSourceType SourceType { get; init; }
        public string Details { get; init; } = string.Empty;

        public MoveSource(int generation, MoveSourceType sourceType, string details = "")
        {
            Generation = generation;
            SourceType = sourceType;
            Details = details;
        }

        public MoveSource(int generation, string sourceType, string details = "")
        {
            Generation = generation;
            SourceType = StringToMoveSourceType(sourceType);
            Details = details;
        }

        public MoveSource(string code)
        {
            string firstChar = code[..1];
            if (!int.TryParse(firstChar, out int generation))
            {
                throw new ArgumentOutOfRangeException(nameof(code), "Invalid generation in move source code.");
            }
            Generation = generation;
            string sourceTypeStr = code.Substring(1, 1);
            SourceType = StringToMoveSourceType(sourceTypeStr);
            Details = code.Length > 2 ? code.Substring(2) : string.Empty;
        }

        public override string ToString()
        {
            return $"{Generation}{MoveSourceTypeToString(SourceType)}{Details}";
        }

        private static string MoveSourceTypeToString(MoveSourceType sourceType)
        {
            return sourceType switch
            {
                MoveSourceType.Tm => "M",
                MoveSourceType.Tutor => "T",
                MoveSourceType.LevelUp => "L",
                MoveSourceType.Restricted => "R",
                MoveSourceType.Egg => "E",
                MoveSourceType.DreamWorld => "D",
                MoveSourceType.Event => "S",
                MoveSourceType.Virtual => "V",
                MoveSourceType.Chain => "C",
                _ => throw new ArgumentOutOfRangeException(nameof(sourceType), sourceType, null)
            };
        }
        private static MoveSourceType StringToMoveSourceType(string sourceType)
        {
            return sourceType switch
            {
                "M" => MoveSourceType.Tm,
                "T" => MoveSourceType.Tutor,
                "L" => MoveSourceType.LevelUp,
                "R" => MoveSourceType.Restricted,
                "E" => MoveSourceType.Egg,
                "D" => MoveSourceType.DreamWorld,
                "S" => MoveSourceType.Event,
                "V" => MoveSourceType.Virtual,
                "C" => MoveSourceType.Chain,
                _ => throw new ArgumentOutOfRangeException(nameof(sourceType), sourceType, null)
            };
        }
    }

    public enum EvoType
    {
        Trade,
        UseItem,
        LevelMove,
        LevelExtra,
        LevelFriendship,
        LevelHold,
        Other,
    }

    public enum EvoRegion
    {
        Alola,
        Galar,
    }

    public class GenderRatio
    {
        public required double M
        {
            get;
            init
            {
                if (value is < 0 or > 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                field = value;
            }
        }

        public double F
        {
            get;
            set
            {
                if (value is < 0 or > 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                field = value;
            }
        }
        public GenderRatio() { }
        public GenderRatio(GenderName gender)
        {
            switch (gender)
            {
                case GenderName.M:
                    M = 1.0;
                    F = 0.0;
                    break;
                case GenderName.F:
                    M = 0.0;
                    F = 1.0;
                    break;
                case GenderName.N:
                    M = 0.0;
                    F = 0.0;
                    break;
                case GenderName.Empty:
                    M = 0.5;
                    F = 0.5;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gender), gender, null);
            }
        }
    }

    public class Species : BasicEffect, IBasicEffect, ISpeciesFormatsData, IEffect
    {
        // ISpeciesFormatsData implementation
        public Tier? DoublesTier
        {
            get;
            set // TierTypes.Doubles | TierTypes.Other
            {
                if (value == null || TierTools.IsDoublesOrOtherTier(value))
                {
                    field = value;
                }
            }
        }

        public bool? GmaxUnreleased
        {
            get => field ?? false;
            init;
        }
        public Tier? NatDexTier
        {
            get;
            set // TierTypes.Singles | TierTypes.Other
            {
                if (value == null || TierTools.IsSinglesOrOtherTier(value))
                {
                    field = value;
                }
            }
        }
        public Tier? Tier
        {
            get;
            set // TierTypes.Singles | TierTypes.Other
            {
                if (value == null || TierTools.IsSinglesOrOtherTier(value))
                {
                    field = value;
                }
            }
        }
        // Species properties
        public required string BaseSpecies
        {
            get => string.IsNullOrEmpty(field) ? Name : field;
            init;
        }
        public required string Forme { get; init; }
        public required string BaseForme { get; init; }
        public List<string>? CosmeticFormes { get; init; }
        public List<string>? OtherFormes { get; init; }
        public List<string>? FormeOrder { get; init; }

        public required string SpriteId
        {
            get => field == string.Empty ? field :
                $"{new Id(BaseSpecies)}{(BaseSpecies != Name ? $"-{new Id(Forme)}" : "")}";
            init;
        }
        public required SpeciesAbility Abilities { get; set; }

        public required List<PokemonType> Types
        {
            get => field.Count == 0 ? [PokemonType.Unknown] : field;
            init;
        }
        public required PokemonType? AddedType { get; init; }
        public required string Prevo { get; init; }
        public required List<string> Evos { get; init; }
        public EvoType? EvoType { get; init; }
        public string? EvoCondition { get; init; }
        public string? EvoItem { get; init; }
        public string? EvoMove { get; init; }
        public EvoRegion? EvoRegion { get; init; }
        public int? EvoLevel { get; init; }
        public required bool Nfe { get; set; }
        public required List<string> EggGroups { get; init; }
        public required bool CanHatch { get; set; }
        public required GenderName Gender { get; init; }
        public required GenderRatio GenderRatio { get; init; }
        public required StatsTable BaseStats { get; init; }
        public int? MaxHp { get; init; }
        public int Bst => BaseStats.BaseStatTotal;
        public required double WeightKg { get; init; }
        public double WeightHg => WeightKg * 10.0;
        public required double HeightM { get; init; }
        public required string Color { get; init; }
        public required List<SpeciesTag> Tags { get; init; }
        public required DexSpeciesUnreleasedHidden UnreleasedHidden { get; init; }
        public required bool MaleOnlyHidden { get; init; }
        public string? Mother { get; init; }

        public bool? IsMega
        {
            get
            {
                if (field != null)
                {
                    return field;
                }
                return Forme is "Mega" or "Mega-X" or "Mega-Y" ? true : null;
            }
            init;
        }

        public bool? IsPrimal
        {
            get => Forme is "Primal" ? true : field;
            init;
        }
        public string? CanGigantamax { get; init; }

        public bool? CannotDynamax
        {
            get => field ?? false;
            init;
        }
        public string? RequiredTeraType { get; init; }

        public StrListStrUnion? BattleOnly { get; init; }
        public string? RequiredItem { get; init; }
        public string? RequiredMove { get; init; }
        public string? RequiredAbility { get; init; }

        public List<string>? RequiredItems
        {
            get
            {
                if (field is { Count: > 0 })
                {
                    return field;
                }
                if (RequiredItem != null)
                {
                    return [RequiredItem];
                }
                return null;
            }
            init;
        }

        public string? ChangesFrom
        {
            get
            {
                if (field != null)
                {
                    return field;
                }
                switch (BattleOnly)
                {
                    case StrStrListStrUnion(var str):
                        if (str != BaseSpecies)
                        {
                            return str;
                        }
                        break;
                    case StrListStrListUnion(var strList):
                        if (strList.Count > 0 && strList[0] != BaseSpecies)
                        {
                            return strList[0];
                        }
                        break;
                }
                return BaseSpecies;
            }
            init;
        }
        public List<string>? PokemonGoData { get; init; }


        public override required string Fullname
        {
            get => $"pokemon: {Name}";
            init { }
        }

        public override required EffectType EffectType
        {
            get => EffectType.Pokemon;
            init { }
        }

        public override required int Gen
        {
            get
            {
                if (field != 0 || Num < 1) return field;
                if (Num >= 906 || (Forme.Contains("Paldea")))
                {
                    return 9;
                }
                if (Num >= 810 || ((Forme == "Gmax" || Forme == "Galar" || Forme == "Galar-Zen" || Forme == "Hisui")))
                {
                    return 8;
                }
                if (Num >= 722 || ((Forme.StartsWith("Alola") || Forme == "Starter")))
                {
                    return 7;
                }
                if (Forme == "Primal")
                {
                    return 6;
                }
                if (Num >= 650 || IsMega == true)
                {
                    return 6;
                }

                return Num switch
                {
                    >= 494 => 5,
                    >= 387 => 4,
                    >= 252 => 3,
                    >= 152 => 2,
                    _ => 1
                };
            }
            init;
        }
        public Dictionary<string, object> ExtraData { get; set; } = [];
        public Action<Battle, Pokemon>? OnAnySwitchIn { get; init; }
        public Action<Battle, Pokemon>? OnSwitchIn { get; init; }
        public Func<Battle, Pokemon, Pokemon, IEffect, bool?>? OnStart { get; init; }
    }

    public static class SpeciesUtils
    {
        //public static Species EmptySpecies => new()
        //{
        //    Fullname = string.Empty,
        //    EffectType = EffectType.Condition,
        //    Gen = 0,
        //    Name = string.Empty,
        //    Exists = false,
        //    Num = 0,
        //    NoCopy = false,
        //    AffectsFainted = false,
        //    SourceEffect = string.Empty,
        //    BaseSpecies = string.Empty,
        //    Forme = string.Empty,
        //    BaseForme = string.Empty,
        //    SpriteId = string.Empty,
        //    Abilities = new SpeciesAbility { Slot0 = string.Empty },
        //    Types = [],
        //    AddedType = null,
        //    Prevo = string.Empty,
        //    Evos = [],
        //    Nfe = false,
        //    EggGroups = [],
        //    CanHatch = false,
        //    Gender = GenderName.M,
        //    GenderRatio = new GenderRatio { M = 0.0, F = 0.0 },
        //    BaseStats = new StatsTable(),
        //    WeightKg = 0,
        //    HeightM = 0,
        //    Color = string.Empty,
        //    Tags = [],
        //    UnreleasedHidden = false,
        //    MaleOnlyHidden = false,
        //    Tier = Tier.Illegal,
        //    DoublesTier = Tier.Illegal,
        //    NatDexTier = Tier.Illegal,
        //    IsNonstandard = Nonstandard.Custom,
        //};

        public static Species EmptySpecies(string? name = null)
        {
            Species emptySpecies = new()
            {
                Fullname = string.Empty,
                EffectType = EffectType.Condition,
                Gen = 0,
                Name = name ?? string.Empty,
                Exists = false,
                Num = 0,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
                BaseSpecies = string.Empty,
                Forme = string.Empty,
                BaseForme = string.Empty,
                SpriteId = string.Empty,
                Abilities = new SpeciesAbility { Slot0 = string.Empty },
                Types = [],
                AddedType = null,
                Prevo = string.Empty,
                Evos = [],
                Nfe = false,
                EggGroups = [],
                CanHatch = false,
                Gender = GenderName.M,
                GenderRatio = new GenderRatio { M = 0.0, F = 0.0 },
                BaseStats = new StatsTable(),
                WeightKg = 0,
                HeightM = 0,
                Color = string.Empty,
                Tags = [],
                UnreleasedHidden = false,
                MaleOnlyHidden = false,
                Tier = Tier.Illegal,
                DoublesTier = Tier.Illegal,
                NatDexTier = Tier.Illegal,
                IsNonstandard = Nonstandard.Custom,
            };
            return emptySpecies;
        }

        public static Species EmptySpecies(Id id)
        {
            return EmptySpecies(id.ToString());
        }
}

    public class Learnset(Species species)
    {
        public static EffectType EffectType => EffectType.Learnset;

        /// <summary>
        /// Maps move IDs to the list of sources for each move.
        /// </summary>
        public Dictionary<string, List<MoveSource>>? LearnsetData { get; init; }

        /// <summary>
        /// True if the only way to get this Pokémon is from events.
        /// </summary>
        public bool EventOnly {get; init; }

        /// <summary>
        /// List of event data for each event.
        /// </summary>
        public List<EventInfo>? EventData { get; init; }
        public List<EventInfo>? Encounters { get; init; }
        public static bool Exists => true;
        public Species Species { get; init; } = species;
    }

    public class DexSpecies(ModdedDex dex)
    {
        private ModdedDex Dex { get; } = dex;
        private Dictionary<Id, Species> SpeciesCache { get; } = [];
        private Dictionary<Id, Learnset> LearnsetCache { get; } = [];
        private Species[]? AllCache { get; } = null;

        public void LoadTestSpecies()
        {
            foreach (var data in Pokedex.SpeciesData)
            {
                SpeciesCache[data.Key] = data.Value;
                LearnsetCache[data.Key] = new Learnset(data.Value);
            }
        }

        public Species Get(string? name)
        {
            var id = Id.Empty;

            if (string.IsNullOrEmpty(name)) return GetById(id);
            name = name.Trim();
            id = new Id(name);

            // Handle special Nidoran gender cases
            if (id.Value != "nidoran") return GetById(id);
            if (name.EndsWith($"♀"))
            {
                id = new Id("nidoranf");
            }
            else if (name.EndsWith($"♂"))
            {
                id = new Id("nidoranm");
            }

            return GetById(id);
        }

        public Species Get(Species? species)
        {
            // If a Species object is passed, return it directly
            return species ??
                   // If null is passed, get empty species
                   GetById(Id.Empty);
        }

        public Species GetById(Id id)
        {
            // Return empty species for empty ID
            if (id.IsEmpty) return SpeciesUtils.EmptySpecies();

            // Check cache first
            if (SpeciesCache.TryGetValue(id, out Species? cachedSpecies))
            {
                return cachedSpecies;
            }

            Species? species = null;

            // Try alias resolution
            species = TryGetSpeciesFromAlias(id);
            if (species != null)
            {
                SpeciesCache[id] = species; // Cache result
                return species;
            }

            // Try forme name parsing if not in Pokedex
            species = TryGetSpeciesFromFormeNames(id);
            if (species != null)
            {
                SpeciesCache[id] = species;
                return species;
            }

            // Try to construct from Pokedex data
            species = TryConstructSpeciesFromPokedex(id);
            if (species != null)
            {
                // Cache successful constructions
                if (species.Exists)
                {
                    SpeciesCache[id] = species;
                }
                return species;
            }

            // Create non-existent species as fallback
            species = SpeciesUtils.EmptySpecies(id);
            return species;
        }

        private Species? TryGetSpeciesFromAlias(Id id)
        {
            Id? alias = Dex.GetAlias(id);
            if (alias is null) return null;

            // Check for special event ID
            if (Dex.Data.FormatsData.TryGetValue(id, out SpeciesFormatsData? formatsData))
            {
                SpeciesData pokedexData = Dex.Data.Pokedex[alias];

                // Create special event species
                Species eventSpecies = CreateSpeciesFromData(pokedexData, formatsData, id.ToString());

                // Special handling for event abilities
                if (pokedexData.Abilities.Special != null)
                {
                    eventSpecies.Abilities = new SpeciesAbility
                    {
                        Slot0 = pokedexData.Abilities.Special
                    };
                }

                return eventSpecies;
            }
            else
            {
                // Regular alias - get the base species
                Species baseSpecies = Get(alias.ToString());

                // Check for cosmetic forme
                if (baseSpecies.CosmeticFormes == null) return baseSpecies;
                foreach (string forme in baseSpecies.CosmeticFormes)
                {
                    if (new Id(forme) == id)
                    {
                        return CreateCosmeticFormSpecies(baseSpecies, forme);
                    }
                }

                return baseSpecies;
            }
        }

        private Species? TryGetSpeciesFromFormeNames(Id id)
        {
            if (Dex.Data.Pokedex.ContainsKey(id)) return null;

            // Forme name mappings
            var formeNames = new Dictionary<string, string[]>
            {
                ["alola"] = ["a", "alola", "alolan"],
                ["galar"] = ["g", "galar", "galarian"],
                ["hisui"] = ["h", "hisui", "hisuian"],
                ["paldea"] = ["p", "paldea", "paldean"],
                ["mega"] = ["m", "mega"],
                ["primal"] = ["p", "primal"]
            };

            foreach (var (forme, aliases) in formeNames)
            {
                foreach (string alias in aliases)
                {
                    string pokeName = "";

                    if (id.Value.StartsWith(alias))
                    {
                        pokeName = id.Value.Substring(alias.Length);
                    }
                    else if (id.Value.EndsWith(alias))
                    {
                        pokeName = id.Value.Substring(0, id.Value.Length - alias.Length);
                    }

                    if (string.IsNullOrEmpty(pokeName)) continue;

                    // Try to resolve pokeName alias
                    Id? resolvedAlias = Dex.GetAlias(new Id(pokeName));
                    pokeName = resolvedAlias?.ToString() ?? pokeName;

                    string fullFormeName = pokeName + forme;
                    if (Dex.Data.Pokedex.ContainsKey(new Id(fullFormeName)))
                    {
                        var formeSpecies = Get(fullFormeName);
                        if (formeSpecies.Exists)
                        {
                            return formeSpecies;
                        }
                    }
                }
            }

            return null;
        }

        private Species? TryConstructSpeciesFromPokedex(Id id)
        {
            if (!Dex.Data.Pokedex.TryGetValue(id, out SpeciesData? pokedexData))
            {
                return null;
            }

            // Get base species tags if applicable
            List<SpeciesTag>? baseSpeciesTags = null;
            if (!string.IsNullOrEmpty(pokedexData.BaseSpecies))
            {
                var baseSpeciesId = new Id(pokedexData.BaseSpecies);
                if (Dex.Data.Pokedex.TryGetValue(baseSpeciesId, out SpeciesData? baseData))
                {
                    baseSpeciesTags = baseData.Tags;
                }
            }

            // Get formats data
            Dex.Data.FormatsData.TryGetValue(id, out SpeciesFormatsData? formatsData);

            // Create species with combined data
            var species = CreateSpeciesFromData(pokedexData, formatsData, id.ToString(), baseSpeciesTags);

            // Inherit status conditions from base species (Arceus, Silvally)
            InheritBaseSpeciesStatuses(species);

            // Apply tier inheritance logic
            ApplyTierInheritance(species);

            // Apply generation-specific restrictions
            ApplyGenerationRestrictions(species);

            // Apply mod-specific restrictions
            ApplyModSpecificRestrictions(species);

            // Calculate NFE (Not Fully Evolved) status
            CalculateNfeStatus(species);

            // Calculate breeding eligibility
            CalculateBreedingEligibility(species);

            // Apply generation-specific stat adjustments
            ApplyGenerationAdjustments(species);

            // Apply ability restrictions for older generations
            ApplyAbilityRestrictions(species);

            // Try to reuse parent mod species if identical
            species = TryReuseParentModSpecies(species, id);

            return species;
        }

        private Species CreateSpeciesFromData(SpeciesData pokedexData, SpeciesFormatsData? formatsData,
            string name, List<SpeciesTag>? tags = null)
        {
            return new Species
            {
                // Copy all required properties from pokedexData
                Name = name,
                Num = pokedexData.Num,
                BaseSpecies = pokedexData.BaseSpecies,
                Forme = pokedexData.Forme,
                BaseForme = pokedexData.BaseForme,
                SpriteId = pokedexData.SpriteId,
                Abilities = pokedexData.Abilities,
                Types = pokedexData.Types,
                AddedType = pokedexData.AddedType,
                Prevo = pokedexData.Prevo,
                Evos = pokedexData.Evos,
                Nfe = pokedexData.Nfe,
                EggGroups = pokedexData.EggGroups,
                CanHatch = pokedexData.CanHatch,
                Gender = pokedexData.Gender,
                GenderRatio = pokedexData.GenderRatio,
                BaseStats = pokedexData.BaseStats,
                WeightKg = pokedexData.WeightKg,
                HeightM = pokedexData.HeightM,
                Color = pokedexData.Color,
                Tags = tags ?? pokedexData.Tags,
                UnreleasedHidden = pokedexData.UnreleasedHidden,
                MaleOnlyHidden = pokedexData.MaleOnlyHidden,

                // Apply formats data if available
                Tier = formatsData?.Tier ?? pokedexData.Tier,
                DoublesTier = formatsData?.DoublesTier ?? pokedexData.DoublesTier,
                NatDexTier = formatsData?.NatDexTier ?? pokedexData.NatDexTier,
                IsNonstandard = formatsData?.IsNonstandard ?? pokedexData.IsNonstandard,
                GmaxUnreleased = formatsData?.GmaxUnreleased ?? pokedexData.GmaxUnreleased,

                // BasicEffect required properties
                Fullname = $"pokemon: {name}",
                EffectType = EffectType.Pokemon,
                Gen = CalculateGeneration(pokedexData),
                Exists = true,
                SourceEffect = string.Empty,
                NoCopy = false,
                AffectsFainted = false
            };
        }

        private Species CreateCosmeticFormSpecies(Species baseSpecies, string forme)
        {
            string formeId = forme.Substring(baseSpecies.Name.Length + 1);

            return new Species
            {
                // Copy most properties from base species
                Name = forme,
                Forme = formeId,
                BaseForme = "",
                BaseSpecies = baseSpecies.Name,
                OtherFormes = null,
                CosmeticFormes = null,

                // Copy all other required properties...
                Num = baseSpecies.Num,
                SpriteId = baseSpecies.SpriteId,
                Abilities = baseSpecies.Abilities,
                Types = baseSpecies.Types,
                AddedType = baseSpecies.AddedType,
                Prevo = baseSpecies.Prevo,
                Evos = baseSpecies.Evos,
                Nfe = baseSpecies.Nfe,
                EggGroups = baseSpecies.EggGroups,
                CanHatch = baseSpecies.CanHatch,
                Gender = baseSpecies.Gender,
                GenderRatio = baseSpecies.GenderRatio,
                BaseStats = baseSpecies.BaseStats,
                WeightKg = baseSpecies.WeightKg,
                HeightM = baseSpecies.HeightM,
                Color = baseSpecies.Color,
                Tags = baseSpecies.Tags,
                UnreleasedHidden = baseSpecies.UnreleasedHidden,
                MaleOnlyHidden = baseSpecies.MaleOnlyHidden,
                Tier = baseSpecies.Tier,
                DoublesTier = baseSpecies.DoublesTier,
                NatDexTier = baseSpecies.NatDexTier,
                IsNonstandard = baseSpecies.IsNonstandard,
                GmaxUnreleased = baseSpecies.GmaxUnreleased,

                // BasicEffect properties
                Fullname = $"pokemon: {forme}",
                EffectType = EffectType.Pokemon,
                Gen = baseSpecies.Gen,
                Exists = true,
                SourceEffect = string.Empty,
                NoCopy = false,
                AffectsFainted = false
            };
        }

        // Helper methods for complex logic
        private void InheritBaseSpeciesStatuses(Species species)
        {
            if (string.IsNullOrEmpty(species.BaseSpecies)) return;

            var baseSpeciesId = new Id(species.BaseSpecies);
            if (Dex.Data.Conditions.TryGetValue(baseSpeciesId, out var baseConditions))
            {
                // Apply base species conditions that don't already exist
                // This would require reflection or a more sophisticated mapping system
                // Implementation depends on your condition system
            }
        }

        private void ApplyTierInheritance(Species species)
        {
            // Skip if all tiers are already set or if this is a base species
            if (species is { Tier: not null, DoublesTier: not null, NatDexTier: not null }) return;
            if (species.BaseSpecies == species.Name) return;

            var baseSpeciesId = new Id(species.BaseSpecies);

            // Apply tier inheritance based on species type
            if (species.BaseSpecies == "Mimikyu")
            {
                ApplyMimikyuTiers(species, baseSpeciesId);
            }
            else if (species.Name.EndsWith("totem"))
            {
                ApplyTotemTiers(species);
            }
            else if (species.BattleOnly != null)
            {
                ApplyBattleOnlyTiers(species);
            }
            else
            {
                ApplyBaseSpeciesTiers(species, baseSpeciesId);
            }

            // Apply defaults for any still-null tiers
            species.Tier ??= Tier.Illegal;
            species.DoublesTier ??= species.Tier;
            species.NatDexTier ??= species.Tier;
        }

        private void ApplyGenerationRestrictions(Species species)
        {
            if (species.Gen <= Dex.Gen) return;
            species.Tier = Tier.Illegal;
            species.DoublesTier = Tier.Illegal;
            species.NatDexTier = Tier.Illegal;
            species.IsNonstandard = Nonstandard.Future;
        }

        private void ApplyModSpecificRestrictions(Species species)
        {
            if (Dex.CurrentMod == "gen7letsgo" && species.IsNonstandard == null)
            {
                ApplyLetsGoRestrictions(species);
            }

            if (Dex.CurrentMod == "gen8bdsp")
            {
                ApplyBdspRestrictions(species);
            }
        }

        private void CalculateNfeStatus(Species species)
        {
            species.Nfe = species.Evos.Any(evo =>
            {
                Species evoSpecies = Get(evo);
                return evoSpecies.IsNonstandard == null ||
                       evoSpecies.IsNonstandard == species.IsNonstandard ||
                       evoSpecies.IsNonstandard == Nonstandard.Unobtainable;
            });
        }

        private static void CalculateBreedingEligibility(Species species)
        {
            species.CanHatch = species.CanHatch ||
                (!species.EggGroups.Contains("Ditto") &&
                 !species.EggGroups.Contains("Undiscovered") &&
                 string.IsNullOrEmpty(species.Prevo) &&
                 species.Name != "Manaphy");
        }

        private void ApplyGenerationAdjustments(Species species)
        {
            // Gen 1: Remove Special Defense from BST calculation
            if (Dex.Gen == 1)
            {
                // Note: In the original code, this modifies species.bst -= species.baseStats.spd
                // Since BST is calculated as a property, we'd need to modify the base stats
                // or handle this in the BST calculation itself
            }
        }

        private void ApplyAbilityRestrictions(Species species)
        {
            if (Dex.Gen < 5)
            {
                // Remove hidden abilities in older generations
                species.Abilities = new SpeciesAbility
                {
                    Slot0 = species.Abilities.Slot0,
                    Slot1 = species.Abilities.Slot1,
                    Hidden = null,
                    Special = species.Abilities.Special
                };
            }

            if (Dex.Gen != 3) return;
            // Remove abilities that don't exist in Gen 3
            Ability slot1Ability = Dex.Abilities.Get(species.Abilities.Slot1 ?? "");
            if (slot1Ability.Gen == 4)
            {
                species.Abilities = new SpeciesAbility
                {
                    Slot0 = species.Abilities.Slot0,
                    Slot1 = null,
                    Hidden = species.Abilities.Hidden,
                    Special = species.Abilities.Special
                };
            }
        }

        private static Species TryReuseParentModSpecies(Species species, Id id)
        {
            return species;
        }

        private static int CalculateGeneration(SpeciesData data)
        {
            // Generation calculation logic from the original Species class
            if (data.Num < 1) return 0;

            if (data.Num >= 906 || data.Forme.Contains("Paldea")) return 9;
            if (data.Num >= 810 || data.Forme is "Gmax" or "Galar" or "Galar-Zen" or "Hisui") return 8;
            if (data.Num >= 722 || data.Forme.StartsWith("Alola") || data.Forme == "Starter") return 7;
            if (data.Forme == "Primal") return 6;
            if (data.Num >= 650) return 6; // Mega evolution era

            return data.Num switch
            {
                >= 494 => 5,
                >= 387 => 4,
                >= 252 => 3,
                >= 152 => 2,
                _ => 1
            };
        }

        private void ApplyMimikyuTiers(Species species, Id baseSpeciesId)
        {
            if (!Dex.Data.FormatsData.TryGetValue(baseSpeciesId, out SpeciesFormatsData? baseFormatsData))
                return;

            species.Tier ??= baseFormatsData.Tier ?? Tier.Illegal;
            species.DoublesTier ??= baseFormatsData.DoublesTier ?? species.Tier;
            species.NatDexTier ??= baseFormatsData.NatDexTier ?? species.Tier;
        }

        private void ApplyTotemTiers(Species species)
        {
            // Remove 'totem' suffix to get base species ID
            string baseId = species.Name.Substring(0, species.Name.Length - 5); // Remove "totem"
            var baseSpeciesId = new Id(baseId);

            if (!Dex.Data.FormatsData.TryGetValue(baseSpeciesId, out SpeciesFormatsData? baseFormatsData))
                return;

            species.Tier ??= baseFormatsData.Tier ?? Tier.Illegal;
            species.DoublesTier ??= baseFormatsData.DoublesTier ?? species.Tier;
            species.NatDexTier ??= baseFormatsData.NatDexTier ?? species.Tier;
        }

        private void ApplyBattleOnlyTiers(Species species)
        {
            if (species.BattleOnly == null) return;

            Id battleOnlyId = species.BattleOnly switch
            {
                StrStrListStrUnion(var str) => new Id(str),
                StrListStrListUnion(var strList) when strList.Count > 0 => new Id(strList[0]),
                _ => Id.Empty
            };

            if (battleOnlyId.IsEmpty) return;

            if (!Dex.Data.FormatsData.TryGetValue(battleOnlyId, out SpeciesFormatsData? battleOnlyFormatsData))
                return;

            species.Tier ??= battleOnlyFormatsData.Tier ?? Tier.Illegal;
            species.DoublesTier ??= battleOnlyFormatsData.DoublesTier ?? species.Tier;
            species.NatDexTier ??= battleOnlyFormatsData.NatDexTier ?? species.Tier;
        }

        private void ApplyBaseSpeciesTiers(Species species, Id baseSpeciesId)
        {
            if (!Dex.Data.FormatsData.TryGetValue(baseSpeciesId, out SpeciesFormatsData? baseFormatsData))
            {
                throw new InvalidOperationException($"{species.BaseSpecies} has no formats-data entry");
            }

            species.Tier ??= baseFormatsData.Tier ?? Tier.Illegal;
            species.DoublesTier ??= baseFormatsData.DoublesTier ?? species.Tier;
            species.NatDexTier ??= baseFormatsData.NatDexTier ?? species.Tier;
        }

        private static void ApplyLetsGoRestrictions(Species species)
        {
            // Check if Pokemon is valid in Let's Go
            bool isLetsGo = (species.Num <= 151 ||
                            new[] { "Meltan", "Melmetal" }.Contains(species.Name)) &&
                           (string.IsNullOrEmpty(species.Forme) ||
                            (new[] { "Alola", "Mega", "Mega-X", "Mega-Y", "Starter" }.Contains(species.Forme) &&
                             species.Name != "Pikachu-Alola"));

            if (!isLetsGo)
            {
                species.IsNonstandard = Nonstandard.Past;
            }
        }

        private static void ApplyBdspRestrictions(Species species)
        {
            // Apply BDSP (Brilliant Diamond/Shining Pearl) restrictions
            if (species.IsNonstandard != null &&
                !new[] { Nonstandard.Gigantamax, Nonstandard.Cap }.Contains(species.IsNonstandard.Value))
                return;

            bool isIllegal = species.Gen > 4 ||
                            (species.Num < 1 && species.IsNonstandard != Nonstandard.Cap) ||
                            species.Name == "Pichu-Spiky-eared";

            if (isIllegal)
            {
                species.IsNonstandard = Nonstandard.Future;
                species.Tier = Tier.Illegal;
                species.DoublesTier = Tier.Illegal;
                species.NatDexTier = Tier.Illegal;
            }
        }

        /**
         * @param id the ID of the species the move pool belongs to
         * @param isNatDex
         * @returns a Set of IDs of the full valid movepool of the given species for the current generation/mod.
         * Note that inter-move incompatibilities, such as those from exclusive events, are not considered and all moves are
         * lumped together. However, Necturna and Necturine's Sketchable moves are omitted from this pool, as their fundamental
         * incompatibility with each other is essential to the nature of those species.
         */
        public List<Id> GetMovePool(Id id, bool isNatDex = false)
        {
            throw new NotImplementedException();
        }

        public List<Learnset> GetFullLearnset(Id id)
        {
            throw new NotImplementedException();
        }

        public Species? LearnsetParent(Species species, bool checkingMoves = false)
        {
            throw new NotImplementedException();
        }

        /**
         * Gets the raw learnset data for the species.
         *
         * In practice, if you're trying to figure out what moves a pokemon learns,
         * you probably want to `getFullLearnset` or `getMovePool` instead.
         */
        public Learnset GetLearnsetData(Id id)
        {
            throw new NotImplementedException();
        }

        public PokemonGoData GetPokemonGoData(Id id)
        {
            throw new NotImplementedException();
        }

        public Species[] All()
        {
            throw new NotImplementedException();
        }

        public bool EggMovesOnly(Species child, Species? father = null)
        {
            throw new NotImplementedException();
        }
    }
}