using System.Collections.Generic;

namespace ApogeeVGC_CS.sim
{
    // SpeciesAbility structure
    public class SpeciesAbility
    {
        public string Slot0 { get; set; } = string.Empty;
        public string? Slot1 { get; set; }
        public string? Hidden { get; set; }
        public string? Special { get; set; }
    }

    // SpeciesTag enum
    public enum SpeciesTag
    {
        Mythical,
        RestrictedLegendary,
        SubLegendary,
        UltraBeast,
        Paradox
    }

    // SpeciesData structure
    public class SpeciesData
    {
        public string Name { get; set; } = string.Empty;
        public int Num { get; set; }
        public List<string> Types { get; set; } = new();
        public SpeciesAbility Abilities { get; set; } = new();
        public StatsTable BaseStats { get; set; } = new();
        public List<string> EggGroups { get; set; } = new();
        public double WeightKg { get; set; }
    }

    // ModdedSpeciesData structure
    public class ModdedSpeciesData : SpeciesData
    {
        public bool Inherit { get; set; }
    }

    // SpeciesFormatsData structure
    public class SpeciesFormatsData
    {
        public string? DoublesTier { get; set; }
        public bool? GmaxUnreleased { get; set; }
        public string? IsNonstandard { get; set; }
        public string? NatDexTier { get; set; }
        public string? Tier { get; set; }
    }

    // ModdedSpeciesFormatsData structure
    public class ModdedSpeciesFormatsData : SpeciesFormatsData
    {
        public bool? Inherit { get; set; }
    }

    // LearnsetData structure
    public class LearnsetData
    {
        public Dictionary<string, List<MoveSource>>? Learnset { get; set; }
        public List<EventInfo>? EventData { get; set; }
        public bool? EventOnly { get; set; }
        public List<EventInfo>? Encounters { get; set; }
        public bool? Exists { get; set; }
    }

    // ModdedLearnsetData structure
    public class ModdedLearnsetData : LearnsetData
    {
        public bool? Inherit { get; set; }
    }

    // PokemonGoData structure
    public class PokemonGoData
    {
        public List<string>? Encounters { get; set; }
        public Dictionary<string, int?>? LGPERestrictiveMoves { get; set; }
    }

    // Data tables
    public class SpeciesDataTable : Dictionary<string, SpeciesData> { }
    public class ModdedSpeciesDataTable : Dictionary<string, ModdedSpeciesData> { }
    public class SpeciesFormatsDataTable : Dictionary<string, SpeciesFormatsData> { }
    public class ModdedSpeciesFormatsDataTable : Dictionary<string, ModdedSpeciesFormatsData> { }
    public class LearnsetDataTable : Dictionary<string, LearnsetData> { }
    public class ModdedLearnsetDataTable : Dictionary<string, ModdedLearnsetData> { }
    public class PokemonGoDataTable : Dictionary<string, PokemonGoData> { }

    public enum MoveSourceType
    {
        TM, Tutor, LevelUp, Restricted, Egg, DreamWorld, Event, Virtual, Chain
    }

    public class MoveSource
    {
        public int Generation { get; set; } // 1-9
        public MoveSourceType SourceType { get; set; }
        public string Details { get; set; } = string.Empty; // Level, event index, etc.

        public MoveSource(int generation, MoveSourceType sourceType, string details = "")
        {
            Generation = generation;
            SourceType = sourceType;
            Details = details;
        }
    }

    public class Species : BasicEffect, ISpecies
    {
        public override EffectType EffectType => EffectType.Pokemon;
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
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
        public bool NFE { get; set; }
        public List<string> EggGroups { get; set; } = new();
        public bool CanHatch { get; set; }
        public string Gender { get; set; } = string.Empty;
        public Dictionary<string, double> GenderRatio { get; set; } = new() { { "M", 0.5 }, { "F", 0.5 } };
        public StatsTable BaseStats { get; set; } = new();
        public int? MaxHP { get; set; }
        public int BST { get; set; }
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
            Name = BasicEffect.GetString(data, "name");
            BaseSpecies = BasicEffect.GetString(data, "baseSpecies") ?? Name;
            Forme = BasicEffect.GetString(data, "forme") ?? string.Empty;
            BaseForme = BasicEffect.GetString(data, "baseForme") ?? string.Empty;
            CosmeticFormes = data.ContainsKey("cosmeticFormes") ? (List<string>?)data["cosmeticFormes"] : null;
            OtherFormes = data.ContainsKey("otherFormes") ? (List<string>?)data["otherFormes"] : null;
            FormeOrder = data.ContainsKey("formeOrder") ? (List<string>?)data["formeOrder"] : null;
            SpriteId = data.ContainsKey("spriteid") ? data["spriteid"]?.ToString() ?? string.Empty :
                (BaseSpecies + (BaseSpecies != Name ? $"-{Forme}" : ""));
            Abilities = data.ContainsKey("abilities") ? (SpeciesAbility)data["abilities"] : new SpeciesAbility();
            Types = data.ContainsKey("types") ? (List<string>)data["types"] : new List<string> { "???" };
            AddedType = data.ContainsKey("addedType") ? data["addedType"]?.ToString() : null;
            Prevo = data.ContainsKey("prevo") ? data["prevo"]?.ToString() ?? string.Empty : string.Empty;
            Tier = data.ContainsKey("tier") ? data["tier"]?.ToString() ?? string.Empty : string.Empty;
            DoublesTier = data.ContainsKey("doublesTier") ? data["doublesTier"]?.ToString() ?? string.Empty : string.Empty;
            NatDexTier = data.ContainsKey("natDexTier") ? data["natDexTier"]?.ToString() ?? string.Empty : string.Empty;
            Evos = data.ContainsKey("evos") ? (List<string>)data["evos"] : new List<string>();
            EvoType = data.ContainsKey("evoType") ? data["evoType"]?.ToString() : null;
            EvoMove = data.ContainsKey("evoMove") ? data["evoMove"]?.ToString() : null;
            EvoLevel = data.ContainsKey("evoLevel") ? (int?)data["evoLevel"] : null;
            NFE = BasicEffect.GetBool(data, "nfe") ?? false;
            EggGroups = data.ContainsKey("eggGroups") ? (List<string>)data["eggGroups"] : new List<string>();
            CanHatch = BasicEffect.GetBool(data, "canHatch") ?? false;
            Gender = data.ContainsKey("gender") ? data["gender"]?.ToString() ?? string.Empty : string.Empty;
            GenderRatio = data.ContainsKey("genderRatio") ? (Dictionary<string, double>)data["genderRatio"] :
                (Gender == "M" ? new() { { "M", 1 }, { "F", 0 } } :
                 Gender == "F" ? new() { { "M", 0 }, { "F", 1 } } :
                 Gender == "N" ? new() { { "M", 0 }, { "F", 0 } } :
                 new() { { "M", 0.5 }, { "F", 0.5 } });
            RequiredItem = data.ContainsKey("requiredItem") ? data["requiredItem"]?.ToString() : null;
            RequiredItems = data.ContainsKey("requiredItems") ? (List<string>?)data["requiredItems"] :
                (RequiredItem != null ? new List<string> { RequiredItem } : null);
            BaseStats = data.ContainsKey("baseStats") ? (StatsTable)data["baseStats"] : new StatsTable();
            BST = BaseStats.hp + BaseStats.atk + BaseStats.def + BaseStats.spa + BaseStats.spd + BaseStats.spe;
            WeightKg = data.ContainsKey("weightkg") ? Convert.ToDouble(data["weightkg"]) : 0;
            WeightHg = WeightKg * 10;
            HeightM = data.ContainsKey("heightm") ? Convert.ToDouble(data["heightm"]) : 0;
            Color = data.ContainsKey("color") ? data["color"]?.ToString() ?? string.Empty : string.Empty;
            Tags = data.ContainsKey("tags") ? (List<SpeciesTag>)data["tags"] : new List<SpeciesTag>();
            UnreleasedHidden = data.ContainsKey("unreleasedHidden") ? data["unreleasedHidden"] : false;
            MaleOnlyHidden = BasicEffect.GetBool(data, "maleOnlyHidden") ?? false;
            MaxHP = data.ContainsKey("maxHP") ? (int?)data["maxHP"] : null;
            IsMega = (Forme != null && (Forme == "Mega" || Forme == "Mega-X" || Forme == "Mega-Y")) ? true : null;
            CanGigantamax = data.ContainsKey("canGigantamax") ? data["canGigantamax"]?.ToString() : null;
            GmaxUnreleased = BasicEffect.GetBool(data, "gmaxUnreleased");
            CannotDynamax = BasicEffect.GetBool(data, "cannotDynamax");
            BattleOnly = data.ContainsKey("battleOnly") ? data["battleOnly"] :
                (IsMega == true ? BaseSpecies : null);
            ChangesFrom = data.ContainsKey("changesFrom") ? data["changesFrom"]?.ToString() :
                (BattleOnly != null && BattleOnly.ToString() != BaseSpecies ? BattleOnly.ToString() : BaseSpecies);
            if (ChangesFrom is List<string> changesList && changesList.Count > 0)
                ChangesFrom = changesList[0];
            PokemonGoData = data.ContainsKey("pokemonGoData") ? (List<string>?)data["pokemonGoData"] : null;

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