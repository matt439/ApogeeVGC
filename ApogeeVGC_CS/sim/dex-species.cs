namespace ApogeeVGC_CS.sim
{
    public class SpeciesAbility(string slot0, string? slot1, string? hidden, string? special)
    {
        public string Slot0 { get; set; } = slot0;
        public string? Slot1 { get; set; } = slot1;
        public string? Hidden { get; set; } = hidden;
        public string? Special { get; set; } = special;

        public SpeciesAbility(SpeciesAbility other) :
            this(other.Slot0, other.Slot1, other.Hidden, other.Special)
        {
        }
    }

    public enum SpeciesTag
    {
        Mythical,
        RestrictedLegendary,
        SubLegendary,
        UltraBeast,
        Paradox
    }

    public interface ISpeciesData
    {
        public string Name { get; set; }
        public int Num { get; set; }
        public List<PokemonType> Types { get; set; }
        public SpeciesAbility Abilities { get; set; }
        public StatsTable BaseStats { get; set; }
        public List<string> EggGroups { get; set; }
        public double WeightKg { get; set; }
    }

    public class SpeciesData(ISpeciesData other) : ISpeciesData
    {
        public string Name { get; set; } = other.Name;
        public int Num { get; set; } = other.Num;
        public List<PokemonType> Types { get; set; } = [.. other.Types];
        public SpeciesAbility Abilities { get; set; } = new(other.Abilities);
        public StatsTable BaseStats { get; set; } = new(other.BaseStats);
        public List<string> EggGroups { get; set; } = [.. other.EggGroups];
        public double WeightKg { get; set; } = other.WeightKg;
    }

    public interface IModdedSpeciesData : ISpeciesData
    {
        public bool Inherit { get; set; }
    }

    public class ModdedSpeciesData(IModdedSpeciesData data) : SpeciesData(data), IModdedSpeciesData
    {
        public bool Inherit { get; set; }
    }

    public interface ISpeciesFormatsData
    {
        public string? DoublesTier { get; set; }
        public bool? GmaxUnreleased { get; set; }
        public string? IsNonstandard { get; set; }
        public string? NatDexTier { get; set; }
        public string? Tier { get; set; }
    }

    public class SpeciesFormatsData : ISpeciesFormatsData
    {
        public string? DoublesTier { get; set; }
        public bool? GmaxUnreleased { get; set; }
        public string? IsNonstandard { get; set; }
        public string? NatDexTier { get; set; }
        public string? Tier { get; set; }
    }

    public class ModdedSpeciesFormatsData : SpeciesFormatsData
    {
        public bool? Inherit { get; set; }
    }

    public class LearnsetData
    {
        public Dictionary<string, List<MoveSource>>? Learnset { get; set; }
        public List<EventInfo>? EventData { get; set; }
        public bool? EventOnly { get; set; }
        public List<EventInfo>? Encounters { get; set; }
        public bool? Exists { get; set; }
    }

    public class ModdedLearnsetData : LearnsetData
    {
        public bool? Inherit { get; set; }
    }

    public class PokemonGoData
    {
        public List<string>? Encounters { get; set; }
        public Dictionary<string, int?>? LgpeRestrictiveMoves { get; set; }
    }

    public class SpeciesDataTable : Dictionary<string, SpeciesData> { }
    public class ModdedSpeciesDataTable : Dictionary<string, ModdedSpeciesData> { }
    public class SpeciesFormatsDataTable : Dictionary<string, SpeciesFormatsData> { }
    public class ModdedSpeciesFormatsDataTable : Dictionary<string, ModdedSpeciesFormatsData> { }
    public class LearnsetDataTable : Dictionary<string, LearnsetData> { }
    public class ModdedLearnsetDataTable : Dictionary<string, ModdedLearnsetData> { }
    public class PokemonGoDataTable : Dictionary<string, PokemonGoData> { }

    public enum MoveSourceType
    {
        Tm, Tutor, LevelUp, Restricted, Egg, DreamWorld, Event, Virtual, Chain
    }

    public class MoveSource(int generation, MoveSourceType sourceType, string details = "")
    {
        public int Generation { get; set; } = generation;
        public MoveSourceType SourceType { get; set; } = sourceType;
        public string Details { get; set; } = details;
    }

    /// <summary>
    /// Represents a species effect.
    /// </summary>
    public interface ISpecies : IEffect { }

    public class Species : BasicEffect, ISpeciesFormatsData, ISpecies
    {
        public string BaseSpecies { get; set; } = string.Empty;
        public string Forme { get; set; } = string.Empty;
        public string BaseForme { get; set; } = string.Empty;
        public List<string>? CosmeticFormes { get; set; }
        public List<string>? OtherFormes { get; set; }
        public List<string>? FormeOrder { get; set; }
        public string SpriteId { get; set; } = string.Empty;
        public SpeciesAbility Abilities { get; set; } = new();
        public List<string> Types { get; set; } = new();
        public string? AddedType { get; set; }
        public string Prevo { get; set; } = string.Empty;
        public List<string> Evos { get; set; } = new();
        public string? EvoType { get; set; }
        public string? EvoCondition { get; set; }
        public string? EvoItem { get; set; }
        public string? EvoMove { get; set; }
        public string? EvoRegion { get; set; }
        public int? EvoLevel { get; set; }
        public bool Nfe { get; set; }
        public List<string> EggGroups { get; set; } = new();
        public bool CanHatch { get; set; }
        public string Gender { get; set; } = string.Empty;
        public Dictionary<string, double> GenderRatio { get; set; } = new() { { "M", 0.5 }, { "F", 0.5 } };
        public StatsTable BaseStats { get; set; } = new();
        public int? MaxHp { get; set; }
        public int Bst { get; set; }
        public double WeightKg { get; set; }
        public double WeightHg { get; set; }
        public double HeightM { get; set; }
        public string Color { get; set; } = string.Empty;
        public List<SpeciesTag> Tags { get; set; } = new();
        public object? UnreleasedHidden { get; set; } // bool or "Past"
        public bool MaleOnlyHidden { get; set; }
        public string? Mother { get; set; }
        public bool? IsMega { get; set; }
        public bool? IsPrimal { get; set; }
        public string? CanGigantamax { get; set; }
        public bool? GmaxUnreleased { get; set; }
        public bool? CannotDynamax { get; set; }
        public string? RequiredTeraType { get; set; }
        public object? BattleOnly { get; set; } // string or string[]
        public string? RequiredItem { get; set; }
        public string? RequiredMove { get; set; }
        public string? RequiredAbility { get; set; }
        public List<string>? RequiredItems { get; set; }
        public string? ChangesFrom { get; set; }
        public List<string>? PokemonGoData { get; set; }
        public string Tier { get; set; } = string.Empty;
        public string DoublesTier { get; set; } = string.Empty;
        public string NatDexTier { get; set; } = string.Empty;
        public int Gen { get; set; }

        public Species(IAnyObject data) : base(data)
        {
            // Generation assignment
            if (Gen == 0 && Num >= 1)
            {
                if (Num >= 906 || (Forme != null && Forme.Contains("Paldea")))
                {
                    Gen = 9;
                }
                else if (Num >= 810 || (Forme != null && (Forme == "Gmax" || Forme == "Galar" || Forme == "Galar-Zen" || Forme == "Hisui")))
                {
                    Gen = 8;
                }
                else if (Num >= 722 || (Forme != null && (Forme.StartsWith("Alola") || Forme == "Starter")))
                {
                    Gen = 7;
                }
                else if (Forme == "Primal")
                {
                    Gen = 6;
                    IsPrimal = true;
                    BattleOnly = BaseSpecies;
                }
                else if (Num >= 650 || IsMega == true)
                {
                    Gen = 6;
                }
                else if (Num >= 494)
                {
                    Gen = 5;
                }
                else if (Num >= 387)
                {
                    Gen = 4;
                }
                else if (Num >= 252)
                {
                    Gen = 3;
                }
                else if (Num >= 152)
                {
                    Gen = 2;
                }
                else
                {
                    Gen = 1;
                }
            }

            // TODO: assignMissingFields logic
        }
    }
}