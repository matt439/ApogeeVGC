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
            init // TierTypes.Doubles | TierTypes.Other
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
        public required SpeciesAbility Abilities { get; init; }

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
        public required bool Nfe { get; init; }
        public required List<string> EggGroups { get; init; }
        public required bool CanHatch { get; init; }
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
        public required object UnreleasedHidden
        {
            get;
            init // bool or "Past"
            {
                field = value switch
                {
                    bool => value,
                    "Past" => true,
                    _ => false
                };
            }
        } 
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

        public List<string>? BattleOnly
        {
            get // string or string[]
            {
                if (field is { Count: > 0 })
                {
                    return field;
                }
                if (IsMega is true || Forme is "Primal")
                {
                    return [BaseSpecies];
                }
                return null;
            }
            init;
        } 
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
                if (BattleOnly is { Count: > 0 } && BattleOnly[0] != BaseSpecies)
                {
                    return BattleOnly[0];
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
    }

    public static class SpeciesConstants
    {
        public static readonly Species EmptySpecies = new()
        {
            Fullname = string.Empty,
            EffectType = EffectType.Condition,
            Gen = 0,
            Name = string.Empty,
            Exists = false,
            Num = 0,
            NoCopy = false,
            AffectsFainted = false,
            SourceEffect = string.Empty,
            BaseSpecies = string.Empty,
            Forme = string.Empty,
            BaseForme = string.Empty,
            SpriteId = string.Empty,
            Abilities = new SpeciesAbility {Slot0 = string.Empty},
            Types = [],
            AddedType = null,
            Prevo = string.Empty,
            Evos = [],
            Nfe = false,
            EggGroups = [],
            CanHatch = false,
            Gender = GenderName.M,
            GenderRatio = new GenderRatio {M = 0.0, F = 0.0},
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
        public required bool EventOnly {get; init; }

        /// <summary>
        /// List of event data for each event.
        /// </summary>
        public List<EventInfo>? EventData { get; init; }
        public List<EventInfo>? Encounters { get; init; }
        public static bool Exists => true;
        public required Species Species { get; init; } = species;
    }

    public class DexSpecies(ModdedDex dex)
    {
        private ModdedDex Dex { get; } = dex;
        private Dictionary<Id, Species> SpeciesCache { get; } = [];
        private Dictionary<Id, Learnset> LearnsetCache { get; } = [];
        private Species[]? AllCache { get; } = null;

        public Species Get(string? name)
        {
            throw new NotImplementedException();
        }

        public Species Get(Species? species)
        {
            throw new NotImplementedException();
        }

        public Species GetById(Id id)
        {
            throw new NotImplementedException();
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