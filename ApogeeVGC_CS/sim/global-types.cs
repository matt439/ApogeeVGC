using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace ApogeeVGC_CS.sim
{
    // An ID must have only lowercase alphanumeric characters
    public class Id
    {
        public string Value
        {
            get;
            set
            {
                if (field == value) return;

                if (IsValid(value))
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("Invalid ID format. Must be lowercase alphanumeric.");
                }
            }
        }

        public virtual bool IsId => true;
        public bool IsEmpty => string.IsNullOrEmpty(Value);
        public static Id Empty => new();

        public Id()
        {
            Value = string.Empty; // Default to empty string
        }

        public Id(string id)
        {
            Value = FromString(id);
        }

        public Id(object obj)
        {
            Value = FromObject(obj);
        }

        public static bool operator ==(Id? left, Id? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Value == right.Value;
        }

        public static bool operator !=(Id? left, Id? right)
        {
            return !(left == right);
        }

        /**
         * Returns true if the sequence of elements of searchString converted to a String is the
         * same as the corresponding elements of this object (converted to a String) starting at
         * endPosition – length(this). Otherwise, returns false.
         */
        public bool EndsWith(string searchString, int? endPosition = null)
        {
            if (string.IsNullOrEmpty(Value) || string.IsNullOrEmpty(searchString))
            {
                return false;
            }
            int startIndex = endPosition.HasValue ? Math.Max(0, endPosition.Value - searchString.Length) : 0;
            return Value.IndexOf(searchString, startIndex, StringComparison.OrdinalIgnoreCase) == startIndex;
        }

        // If a string is passed, it will be converted to lowercase and non-alphanumeric characters will be stripped.
        private static string FromString(string id)
        {
            return Regex.Replace(id.ToLowerInvariant(), @"[^a-z0-9]+", "");
        }

        // If an object with an ID is passed, its ID will be returned. Otherwise, an empty string will be returned.
        private static string FromObject(object obj)
        {
            Type type = obj.GetType();
            PropertyInfo? idProp = type.GetProperty("id") ?? type.GetProperty("userId") ?? type.GetProperty("roomId");

            if (idProp != null && idProp.GetValue(obj) is string propValue)
            {
                return FromString(propValue);
            }
            else
            {
                return string.Empty; // Return empty string if no valid ID found
            }
        }

        public static bool IsValid(string id)
        {
            //if (string.IsNullOrEmpty(id)) return false;
            foreach (char c in id)
            {
                if (!char.IsLower(c) && !char.IsDigit(c)) return false;
            }
            return true;
        }

        public override string ToString()
        {
            return Value;
        }

        public static bool operator ==(Id id, string str)
        {
            if (id is null && str == null) return true;
            if (id is null || str == null) return false;
            return id.Value == FromString(str);
        }

        public static bool operator !=(Id id, string str) => !(id == str);

        public override bool Equals(object? obj)
        {
            return obj switch
            {
                Id otherId => Value == otherId.Value,
                string str => Value == FromString(str),
                _ => false
            };
        }

        public override int GetHashCode() => Value.GetHashCode();

    }

    // must be lowercase alphanumeric
    public class IdEntry : Id
    {
        public override bool IsId => false;

        public IdEntry() { }

        public IdEntry(string id) : base(id) { }

        public IdEntry(object obj) : base(obj) { }
    }

    // must be lowercase alphanumeric and can be empty
    public class PokemonSlot
    {
        public static bool IsSlot => true;

        public string Value
        {
            get;
            set
            {
                if (field == value) return;
                if (IsValid(value))
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("Invalid slot format. Must be lowercase alphanumeric.");
                }
            }
        } = string.Empty;

        public PokemonSlot() { }

        public PokemonSlot(string value)
        {
            Value = value;
        }

        private static bool IsValid(string s)
        {
            return s.Length == 0 || // Empty string is valid
                   Id.IsValid(s);
        }
    }

    public enum GenderName
    {
        M, F, N, Empty
    }

    public enum StatIdExceptHp
    {
        Atk, Def, Spa, Spd, Spe
    }

    public enum StatId
    {
        Hp, Atk, Def, Spa, Spd, Spe
    }

    public static class StatIdTools
    {
        public static StatId ConvertToStatId(StatIdExceptHp stat)
        {
            return stat switch
            {
                StatIdExceptHp.Atk => StatId.Atk,
                StatIdExceptHp.Def => StatId.Def,
                StatIdExceptHp.Spa => StatId.Spa,
                StatIdExceptHp.Spd => StatId.Spd,
                StatIdExceptHp.Spe => StatId.Spe,
                _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID except HP.")
            };
        }

        public static StatIdExceptHp ConvertToStatIdExceptId(StatId stat)
        {
            return stat switch
            {
                StatId.Atk => StatIdExceptHp.Atk,
                StatId.Def => StatIdExceptHp.Def,
                StatId.Spa => StatIdExceptHp.Spa,
                StatId.Spd => StatIdExceptHp.Spd,
                StatId.Spe => StatIdExceptHp.Spe,
                StatId.Hp => throw new ArgumentOutOfRangeException(nameof(stat),
                    "Cannot convert HP to StatIdExceptHp."),
                _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID.")
            };
        }
    }

    public class StatsExceptHpTable : Dictionary<StatIdExceptHp, int>;

    //public class StatsTable : Dictionary<StatId, int>
    //{
    //    public StatsTable()
    //    {
    //        this[StatId.Hp] = 0; // Initialize HP to 0
    //    }
    //    public StatsTable(StatsTable other) : this()
    //    {
    //        foreach (var kvp in other)
    //        {
    //            this[kvp.Key] = kvp.Value;
    //        }
    //    }
    //}

    public class StatsTable
    {
        public int Hp
        {
            get;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("value must be positive.");
                }

                field = value;
            }
        } 
        public int Atk
        {
            get;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("value must be positive.");
                }
                field = value;
            }
        } = 0;
        public int Def
        {
            get;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("value must be positive.");
                }
                field = value;
            }
        } = 0;
        public int Spa
        {
            get;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("value must be positive.");
                }
                field = value;
            }
        } = 0;
        public int Spd
        {
            get;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("value must be positive.");
                }
                field = value;
            }
        } = 0;
        public int Spe
        {
            get;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("value must be positive.");
                }
                field = value;
            }
        } = 0;

        public int BaseStatTotal => Hp + Atk + Def + Spa + Spd + Spe;

        public int GetStat(StatId stat)
        {
            return stat switch
            {
                StatId.Hp => Hp,
                StatId.Atk => Atk,
                StatId.Def => Def,
                StatId.Spa => Spa,
                StatId.Spd => Spd,
                StatId.Spe => Spe,
                _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID.")
            };
        }

        public void SetStat(StatId stat, int value)
        {
            switch (stat)
            {
                case StatId.Hp:
                    Hp = value;
                    break;
                case StatId.Atk:
                    Atk = value;
                    break;
                case StatId.Def:
                    Def = value;
                    break;
                case StatId.Spa:
                    Spa = value;
                    break;
                case StatId.Spd:
                    Spd = value;
                    break;
                case StatId.Spe:
                    Spe = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID.");
            }
        }

        public static bool IsValidIv(int stat)
        {
            return stat is >= 0 and <= 31;
        }

        public static bool IsValidEv(int stat)
        {
            return stat is >= 0 and <= 255;
        }

        public bool IsValidIvs()
        {
            return IsValidIv(Hp) && IsValidIv(Atk) && IsValidIv(Def) &&
                   IsValidIv(Spa) && IsValidIv(Spd) && IsValidIv(Spe);
        }

        public bool IsValidEvs()
        {
            return IsValidEv(Hp) && IsValidEv(Atk) && IsValidEv(Def) &&
                   IsValidEv(Spa) && IsValidEv(Spd) && IsValidEv(Spe);
        }

        public static StatsTable PerfectIvs => new()
        {
            Hp = 31,
            Atk = 31,
            Def = 31,
            Spa = 31,
            Spd = 31,
            Spe = 31,
        };

        public StatsTable() { }

        public StatsTable(StatsTable other)
        {
            Hp = other.Hp;
            Atk = other.Atk;
            Def = other.Def;
            Spa = other.Spa;
            Spd = other.Spd;
            Spe = other.Spe;
        }
    }

    public class SparseStatsTable : Dictionary<StatId, int>
    {
    }

    public enum BoostId
    {
        Atk, Def, Spa, Spd, Spe, Accuracy, Evasion
    }

    public class BoostsTable : Dictionary<BoostId, int>;

    public class SparseBoostsTable : Dictionary<BoostId, int>;

    public enum Nonstandard
    {
        Past, Future, Unobtainable, Cap, Lgpe, Custom, Gigantamax
    }

    //public enum SinglesTier
    //{
    //    Ag, Uber, UberAlt, Ou, OuAlt, Uubl, Uu, Rubl, Ru, Nubl, Nu, NuAlt, Publ, Pu,
    //    PuAlt, Zubl, Zu, Nfe, Lc
    //}

    //public enum DoublesTier
    //{
    //    DUber, DUberAlt, Dou, DouAlt, Dbl, Duu, DuuAlt, Nfe, Lc
    //}

    //public enum OtherTier
    //{
    //    Unreleased, Illegal, Cap, CapNfe, CapLc
    //}

    public enum Tier
    {
        // Singles
        Ag, Uber, UberAlt, Ou, OuAlt, Uubl, Uu, Rubl, Ru, Nubl, Nu, NuAlt, Publ, Pu, PuAlt, Zubl, Zu, Nfe, Lc,
        // Doubles
        DUber, DUberAlt, Dou, DouAlt, Dbl, Duu, DuuAlt,
        // Other
        Unreleased, Illegal, Cap, CapNfe, CapLc
    }

    // Helper class for tier checks
    public static class TierTools
    {
        public static bool IsSinglesTier(Tier? tier)
        {
            return tier is >= Tier.Ag and <= Tier.Lc;
        }
        public static bool IsDoublesTier(Tier? tier)
        {
            return tier is >= Tier.DUber and <= Tier.DuuAlt;
        }
        public static bool IsOtherTier(Tier? tier)
        {
            return tier is >= Tier.Unreleased and <= Tier.CapLc;
        }
        public static bool IsDoublesOrOtherTier(Tier? tier)
        {
            return IsDoublesTier(tier) || IsOtherTier(tier);
        }
        public static bool IsSinglesOrOtherTier(Tier? tier)
        {
            return IsSinglesTier(tier) || IsOtherTier(tier);
        }
    }

    public class EventInfo
    {
        public int Generation { get; set; }
        public int? Level { get; set; }
        public bool? Shiny { get; set; } // true: always shiny, 1: sometimes shiny, false/null: never shiny
        public int? ShinySometimes { get; set; } // Use this if you need to distinguish '1'
        public GenderName? Gender { get; set; }
        public string? Nature { get; set; }
        public SparseStatsTable? Ivs { get; set; }
        public int? PerfectIvs { get; set; }
        public bool? IsHidden { get; set; }
        public List<IdEntry>? Abilities { get; set; }
        public int? MaxEggMoves { get; set; }
        public List<IdEntry>? Moves { get; set; }
        public IdEntry? Pokeball { get; set; }
        public string? From { get; set; }
        public bool? Japan { get; set; }
        public bool? EmeraldEventEgg { get; set; }
    }

    /// <summary>
    /// Ability | Item | ActiveMove | Species | Condition | Format
    /// </summary>
    public interface IEffect
    {
        public EffectType EffectType { get; }
        public Dictionary<string, object> ExtraData { get; set; }
        public string Name { get; }
        public Id Id { get; }
        public int? Duration { get; }
        Action<Battle, Pokemon>? OnAnySwitchIn { get; }
        Action<Battle, Pokemon>? OnSwitchIn { get; }
        public Func<Battle, Pokemon, Pokemon, IEffect, bool?>? OnStart { get; }
    }

    public interface ICommonHandlers
    {
        public Func<Battle, int, Pokemon, Pokemon, IEffect, int?> ModifierEffect { get; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?> ModifierMove { get; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?> ResultMove { get; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolUnion?> ExtResultMove { get; }
        public Action<Battle, Pokemon, Pokemon, IEffect> VoidEffect { get; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove> VoidMove { get; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, int?> ModifierSourceEffect { get; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?> ModifierSourceMove { get; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?> ResultSourceMove { get; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolUnion?> ExtResultSourceMove { get; }
        public Action<Battle, Pokemon, Pokemon, IEffect> VoidSourceEffect { get; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove> VoidSourceMove { get; }
    }

    public interface IEffectData
    {
        public string Name { get; } // changed this from nullable to non-nullable so it matches IBasicEffect
        public string? Desc { get; }
        public int? Duration { get; }
        public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; }
        public EffectType EffectType { get; }
        public bool? Infiltrates { get; }
        public Nonstandard? IsNonstandard { get; }
        public string? ShortDesc { get; }
    }

    public interface IModdedEffectData : IEffectData
    {
        public bool Inherit { get; }
    }

    public enum EffectType
    {
        Condition, Pokemon, Move, Item, Ability, Format,
        Nature, Ruleset, Weather, Status, Terrain, Rule, ValidatorRule,
        Learnset // Added for Learnset class
    }

    public interface IBasicEffect : IEffectData
    {
        public Id Id { get; }
        public bool Exists { get; }
        public string Fullname { get; }
        public int Gen { get; }
        public string SourceEffect { get; }

        //public bool NoCopy { get; }
        //public bool AffectsFainted { get; }
        //public Id? Status { get; }
        //public Id? Weather { get; }
        //public int Num { get; }
        //public string? RealMove { get; }// Added this for the Init method
    }

    public enum GameType
    {
        Singles, Doubles, Triples, Rotation, Multi, FreeForAll
    }

    public enum SideId
    {
        P1 = 0,
        P2 = 1,
        P3 = 2,
        P4 = 3,
    }

    public class SpreadMoveTargets : List<Pokemon?>;
    public class SpreadMoveDamage : List<IntBoolUnion?>;
    public class ZMoveOptions : List<ZMoveOption?>;

    // helper class for ZMoveOptions
    public class ZMoveOption
    {
        public string Move { get; set; } = string.Empty;
        public MoveTarget Target { get; set; }
    }

    public class BattleScriptsData
    {
        public int Gen { get; set; }
    }

    public class RunMoveOptions
    {
        public IEffect? SourceEffect { get; init; }
        public string? ZMove { get; init; }
        public bool? ExternalMove { get; init; }
        public string? MaxMove { get; init; }
        public Pokemon? OriginalTarget { get; init; }
    }

    public interface IModdedBattleActions
    {
        public bool? Inherit { get; set; }
        public Action<BattleActions, List<Pokemon>, Pokemon, ActiveMove>? AfterMoveSecondaryEvent { get; set; }
        public Func<BattleActions, int, Move, Pokemon, int>? CalcRecoilDamage { get; set; }
        public Func<BattleActions, Pokemon, string?>? CanMegaEvo { get; set; }
        public Func<BattleActions, Pokemon, string?>? CanMegaEvoX { get; set; }
        public Func<BattleActions, Pokemon, string?>? CanMegaEvoY { get; set; }
        public Func<BattleActions, Pokemon, string?>? CanTerastallize { get; set; }
        public Func<BattleActions, Pokemon, string?>? CanUltraBurst { get; set; }
        public Func<BattleActions, Pokemon, ZMoveOptions?>? CanZMove { get; set; }
        public Func<BattleActions, SpreadMoveDamage, SpreadMoveTargets, Pokemon, ActiveMove, ActiveMove, bool?, bool?, SpreadMoveDamage>? ForceSwitch { get; set; }
        public Func<BattleActions, Move, Pokemon, ActiveMove>? GetActiveMaxMove { get; set; }
        public Func<BattleActions, Move, Pokemon, ActiveMove>? GetActiveZMove { get; set; }
        public Func<BattleActions, Move, Pokemon, Move?>? GetMaxMove { get; set; }
        public Func<BattleActions, SpreadMoveDamage, SpreadMoveTargets, Pokemon, ActiveMove, ActiveMove, bool?, bool?, SpreadMoveDamage>? GetSpreadDamage { get; set; }
        public Func<BattleActions, Move, Pokemon, bool, string?>? GetZMove { get; set; }
        public Func<BattleActions, List<Pokemon>, Pokemon, ActiveMove, List<bool>>? HitStepAccuracy { get; set; }
        public Action<BattleActions, List<Pokemon>, Pokemon, ActiveMove>? HitStepBreakProtect { get; set; }
        public Func<BattleActions, List<Pokemon>, Pokemon, ActiveMove, SpreadMoveDamage>? HitStepMoveHitLoop { get; set; }
        public Func<BattleActions, List<Pokemon>, Pokemon, ActiveMove, List<bool>>? HitStepTryImmunity { get; set; }
        public Action<BattleActions, List<Pokemon>, Pokemon, ActiveMove>? HitStepStealBoosts { get; set; }
        public Func<BattleActions, List<Pokemon>, Pokemon, ActiveMove, List<bool?>>? HitStepTryHitEvent { get; set; }
        public Func<BattleActions, List<Pokemon>, Pokemon, ActiveMove, List<bool>>? HitStepInvulnerabilityEvent { get; set; }
        public Func<BattleActions, List<Pokemon>, Pokemon, ActiveMove, List<bool>>? HitStepTypeImmunity { get; set; }
        public Func<BattleActions, Pokemon?, Pokemon, ActiveMove, ActiveMove?, bool?, bool?, IntFalseUnion?>? MoveHit { get; set; }
        public Action<BattleActions, IAction>? RunAction { get; set; }
        public Func<BattleActions, Pokemon, bool>? RunMegaEvo { get; set; }
        public Func<BattleActions, Pokemon, bool>? RunMegaEvoX { get; set; }
        public Func<BattleActions, Pokemon, bool>? RunMegaEvoY { get; set; }
        public Action<BattleActions, MoveStringUnion, Pokemon, int, RunMoveOptions?>? RunMove { get; set; }
        public Func<BattleActions, SpreadMoveDamage, SpreadMoveTargets, Pokemon, ActiveMove, ActiveMove, bool?, bool?, SpreadMoveDamage>? RunMoveEffects { get; set; }
        public Func<BattleActions, Pokemon, bool>? RunSwitch { get; set; }
        public Action<BattleActions, ActiveMove, Pokemon>? RunZPower { get; set; }
        public Action<BattleActions, SpreadMoveTargets, Pokemon, ActiveMove, ActiveMove, bool?>? Secondaries { get; set; }
        public Action<BattleActions, SpreadMoveTargets, Pokemon, ActiveMove, ActiveMove, bool?, bool?>? SelfDrops { get; set; }
        public Func<BattleActions, SpreadMoveTargets, Pokemon, ActiveMove, ActiveMove?, bool?, bool?, Tuple<SpreadMoveDamage, SpreadMoveTargets>>? SpreadMoveHit { get; set; }
        public Func<BattleActions, Pokemon, int, IEffect?, bool?, SwitchInReturnBase>? SwitchIn { get; set; }
        public Func<BattleActions, string, bool>? TargetTypeChoices { get; set; }
        public Action<BattleActions, Pokemon>? Terastallize { get; set; }
        public Func<BattleActions, Pokemon, Pokemon, ActiveMove, TryMoveHitReturn?>? TryMoveHit { get; set; }
        public Func<BattleActions, SpreadMoveDamage, SpreadMoveTargets, Pokemon, ActiveMove, ActiveMove, bool?, SpreadMoveDamage>? TryPrimaryHitEvent { get; set; }
        public Func<BattleActions, List<Pokemon>, Pokemon, ActiveMove, bool?, bool>? TrySpreadMoveHit { get; set; }
        public Func<BattleActions, Move, Pokemon, IEffect?, bool>? UseMove { get; set; }
        public Func<BattleActions, Move, Pokemon, IEffect?, bool>? UseMoveInner { get; set; }
        public Func<BattleActions, Pokemon, Pokemon, GetDamageMove, bool, IntFalseUnion?>? GetDamage { get; set; }
        public Action<BattleActions, int, Pokemon, Pokemon, ActiveMove, bool?>? ModifyDamage { get; set; }

        // OMs (Other Metagames)
        public Func<BattleActions, Species, AnyObject, Species>? MutateOriginalSpecies { get; set; }
        public Func<BattleActions, Species, Pokemon?, AnyObject>? GetFormeChangeDeltas { get; set; }
        public Func<BattleActions, string, string, Pokemon?, Species>? GetMixedSpecies { get; set; }
    }

    public class GetRequestDataReturn
    {
        public required string Name { get; init; }
        public required Id Id { get; init; }
        public required List<AnyObject> Pokemon { get; init; }
    }

    public interface IModdedBattleSide
    {
        public bool? Inherit { get; set; }
        public Func<Side, ConditionStrUnion, AddSideConditionSource?, IEffect?, bool>?  AddSideCondition { get; set; }
        public Func<Side, bool?, List<Pokemon>>? Allies { get; set; }
        public Func<Side, bool>? CanDynamaxNow { get; set; }
        public Func<Side, string?, object>? ChooseSwitch { get; set; }
        public Func<Side, string>? GetChoice { get; set; }
        public Func<Side, bool?, GetRequestDataReturn>? GetRequestData { get; set; }
    }

    // Helper class for move choices
    public class MoveChoice
    {
        public string Move { get; init; } = string.Empty;
        public Id Id { get; init; } = new();
        public string? Target { get; init; }
        public bool? Disabled { get; init; }
    }

    // Helper class for GetMoveRequestData
    public class GetMoveRequestDataReturn
    {
        public List<MoveChoice> Moves { get; init; } = [];
        public bool? MaybeDisabled { get; init; }
        public bool? Trapped { get; init; }
        public bool? MaybeTrapped { get; init; }
        public bool? CanMegaEvo { get; init; }
        public bool? CanUltraBurst { get; init; }
        public ZMoveOptions? CanZMove { get; init; }
    }

    // helper class for GetMoves
    public class GetMovesReturn
    {
        public string Move { get; init; } = string.Empty;
        public string Id { get; init; } = string.Empty;
        public BoolStringUnion? Disabled { get; init; }
        public string? DisabledSource { get; init; }
        public string? Target { get; init; }
        public int? Pp { get; init; }
        public int? MaxPp { get; init; }
    }

    // helper class for GetMoveTargets
    public class GetMoveTargetsReturn
    {
        public List<Pokemon> Targets { get; init; } = [];
        public List<Pokemon> PressureTargets { get; init; } = [];
    }

    public interface IModdedBattlePokemon
    {
        public bool? Inherit { get; set; }
        public Item? LostItemForDelibird { get; set; }
        public Func<Pokemon, SparseBoostsTable, IntBoolUnion>? BoostBy { get; set; }
        public Action<Pokemon>? ClearBoosts { get; set; }
        public Func<Pokemon, StatIdExceptHp, int, int?, int>? CalculateStat { get; set; }
        public Func<Pokemon, bool?, bool>? CureStatus { get; set; }
        public Func<Pokemon, MoveStringUnion, int?, SpreadDamageTarget, int>? DeductPp { get; set; }
        public Func<Pokemon, bool?, Pokemon?, IEffect?, bool>? EatItem { get; set; }
        public Func<Pokemon, Id>? EffectiveWeather { get; set; }
        public Func<Pokemon, SpeciesStrUnion, IEffect, bool?, string?, bool>? FormeChange { get; set; }
        public Func<Pokemon, StrListStrUnion, bool>? HasType { get; set; }
        public Func<Pokemon, Ability>? GetAbility { get; set; }
        public Func<Pokemon, int>? GetActionSpeed { get; set; }
        public Func<Pokemon, Item>? GetItem { get; set; }
        public Func<Pokemon, GetMoveRequestDataReturn>? GetMoveRequestData { get; set; }
        public Func<Pokemon, string?, bool?, List<GetMovesReturn>>? GetMoves { get; set; }
        public Func<Pokemon, ActiveMove, Pokemon, GetMoveTargetsReturn>? GetMoveTargets { get; set; }
        public Func<Pokemon, StatIdExceptHp, bool?, bool?, bool?, int>? GetStat { get; set; }
        public Func<Pokemon, bool?, bool?, List<string>>? GetTypes { get; set; }
        public Func<Pokemon, int>? GetWeight { get; set; }
        public Func<Pokemon, StrListStrUnion, bool>? HasAbility { get; set; }
        public Func<Pokemon, StrListStrUnion, bool>? HasItem { get; set; }
        public Func<Pokemon, bool?, bool?>? IsGrounded { get; set; }
        public Action<Pokemon, StatIdExceptHp, int>? ModifyStat { get; set; }
        public Action<Pokemon, ActiveMove, int?>? MoveUsed { get; set; }
        public Action<Pokemon>? RecalculateStats { get; set; }
        public Func<Pokemon, ActiveMove, int>? RunEffectiveness { get; set; }
        public Func<Pokemon, ActiveMoveStringUnion, BoolStringUnion?, bool>? RunImmunity { get; set; }
        public Func<Pokemon, AbilityStringUnion, Pokemon?, bool, StringFalseUnion>? SetAbility { get; set; }
        public Func<Pokemon, ItemStringUnion, Pokemon?, IEffect?, bool>? SetItem { get; set; }
        public Func<Pokemon, ConditionStrUnion, Pokemon?, IEffect?, bool, bool>? SetStatus { get; set; }
        public Func<Pokemon, Pokemon?, ItemBoolUnion>? TakeItem { get; set; }
        public Func<Pokemon, Pokemon, IEffect?, bool>? TransformInto { get; set; }
        public Func<Pokemon, Pokemon?, IEffect?, bool>? UseItem { get; set; }
        public Func<Pokemon, bool>? IgnoringAbility { get; set; }
        public Func<Pokemon, bool>? IgnoringItem { get; set; }
        // OMs (Other Metagames)
        public Func<Pokemon, bool?, List<string>>? GetLinkedMoves { get; set; }
        public Func<Pokemon, string, bool>? HasLinkedMove { get; set; }
    }

    public interface IModdedBattleQueue
    {
        public bool? Inherit { get; set; }
        public Func<BattleQueue, ActionChoice, bool?, List<Action>>? ResolveAction { get; set; }
    }

    public interface IModdedField
    {
        public bool? Inherit { get; set; }
        public Func<Field, bool>? SuppressingWeather { get; set; }
        public Func<Field, ConditionStrUnion, AddSideConditionSource?, IEffect?, bool>? AddPseudoWeather { get; set; }
        public Func<Field, ConditionStrUnion, AddSideConditionSource?, IEffect?, bool?>? SetWeather { get; set; }
        public Func<Field, EffectStringUnion, AddSideConditionSource, IEffect?, bool>? SetTerrain { get; set; }
    }

    public interface IModdedBattleScriptsData
    {
        public string? Inherit { get; set; }
        public IModdedBattleActions? Actions { get; set; }
        public IModdedBattlePokemon? Pokemon { get; set; }
        public IModdedBattleQueue? Queue { get; set; }
        public IModdedField? Field { get; set; }
        public IModdedBattleSide? Side { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, Pokemon?, IEffect?, bool?, bool?, BoolZeroUnion?>? Boost { get; set; }
        public Action<Battle, string>? Debug { get; set; }
        public Action<Battle, AnyObject>? GetActionSpeed { get; set; }
        public Action<ModdedDex>? Init { get; set; }
        public Func<Battle, bool[], string[], bool?>? MaybeTriggerEndlessBattleClause { get; set; }
        public Func<Battle, StatsTable, PokemonSet, StatsTable>? NatureModify { get; set; }
        public Action<Battle>? EndTurn { get; set; }
        public Action<Battle, IAction>? RunAction { get; set; }
        public Func<Battle, StatsTable, PokemonSet, StatsTable>? SpreadModify { get; set; }
        public Action<Battle>? Start { get; set; }
        public Func<Battle, bool>? SuppressingWeather { get; set; }
        public Func<double, double>? Trunc { get; set; }
        public Func<Battle, WinSideUnion?, bool>? Win { get; set; }
        public Func<Battle, bool?, bool?, bool?, bool?>? FaintMessages { get; set; }
        public Func<Battle, bool>? Tiebreak { get; set; }
        public Func<Battle, ActiveMove, Pokemon, Pokemon, bool?, bool>? CheckMoveMakesContact { get; set; }
        public Func<Battle, FaintQueueEntry?, bool?>? CheckWin { get; set; }
        public Action<Battle, string, List<Pokemon>?>? FieldEvent { get; set; }
        public Func<Battle, bool?, bool?, List<Pokemon>>? GetAllActive { get; set; }
    }

    public class PlayerOptions
    {
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public int? Rating { get; set; }
        public PlayerOptionsTeam? Team { get; set; }
        public string? TeamString { get; set; }
        public PrngSeed? Seed { get; set; }
    }
    public interface IBasicTextData
    {
        public string? Desc { get; set; }
        public string? ShortDesc { get; set; }
    }

    public interface IConditionTextData : IBasicTextData
    {
        public string? Activate { get; set; }
        public string? AddItem { get; set; }
        public string? Block { get; set; }
        public string? Boost { get; set; }
        public string? Cant { get; set; }
        public string? ChangeAbility { get; set; }
        public string? Damage { get; set; }
        public string? End { get; set; }
        public string? Heal { get; set; }
        public string? Move { get; set; }
        public string? Start { get; set; }
        public string? Transform { get; set; }
    }

    public interface IMoveTextData : IConditionTextData
    {
        public string? AlreadyStarted { get; set; }
        public string? BlockSelf { get; set; }
        public string? ClearBoost { get; set; }
        public string? EndFromItem { get; set; }
        public string? Fail { get; set; }
        public string? FailSelect { get; set; }
        public string? FailTooHeavy { get; set; }
        public string? FailWrongForme { get; set; }
        public string? MegaNoItem { get; set; }
        public string? Prepare { get; set; }
        public string? RemoveItem { get; set; }
        public string? StartFromItem { get; set; }
        public string? StartFromZEffect { get; set; }
        public string? SwitchOut { get; set; }
        public string? TakeItem { get; set; }
        public string? TypeChange { get; set; }
        public string? Upkeep { get; set; }
    }

    // Interface for generation-specific text data
    public interface IGenerationTextData
    {
        string? Desc { get; }
        string? ShortDesc { get; }
    }

    // Example implementation for generation-specific text
    public class GenerationTextData : IGenerationTextData
    {
        public string? Desc { get; init; }
        public string? ShortDesc { get; init; }
    }


    public class TextFile<T> where T : class
    {
        public string Name { get; set; } = string.Empty;
        public T? Gen1 { get; set; }
        public T? Gen2 { get; set; }
        public T? Gen3 { get; set; }
        public T? Gen4 { get; set; }
        public T? Gen5 { get; set; }
        public T? Gen6 { get; set; }
        public T? Gen7 { get; set; }
        public T? Gen8 { get; set; }
    }

    public class AbilityText : TextFile<AbilityTextData>, ITextFile
    {
        public string? Desc { get; init; }
        public string? ShortDesc { get; init; }

        // Generation-specific properties
        public GenerationTextData? Gen3 { get; init; }
        public GenerationTextData? Gen4 { get; init; }
        public GenerationTextData? Gen5 { get; init; }
        public GenerationTextData? Gen6 { get; init; }
        public GenerationTextData? Gen7 { get; init; }
        public GenerationTextData? Gen8 { get; init; }
        public GenerationTextData? Gen9 { get; init; }
    }

    public class MoveText : TextFile<MoveTextData>, ITextFile
    {
        public string? Desc { get; init; }
        public string? ShortDesc { get; init; }
        // Generation-specific properties
        public GenerationTextData? Gen3 { get; init; }
        public GenerationTextData? Gen4 { get; init; }
        public GenerationTextData? Gen5 { get; init; }
        public GenerationTextData? Gen6 { get; init; }
        public GenerationTextData? Gen7 { get; init; }
        public GenerationTextData? Gen8 { get; init; }
        public GenerationTextData? Gen9 { get; init; }
    }

    public class ItemText : TextFile<ItemTextData>, ITextFile
    {
        public string? Desc { get; init; }
        public string? ShortDesc { get; init; }
        // Generation-specific properties
        public GenerationTextData? Gen3 { get; init; }
        public GenerationTextData? Gen4 { get; init; }
        public GenerationTextData? Gen5 { get; init; }
        public GenerationTextData? Gen6 { get; init; }
        public GenerationTextData? Gen7 { get; init; }
        public GenerationTextData? Gen8 { get; init; }
        public GenerationTextData? Gen9 { get; init; }
    }

    public class PokedexText : TextFile<PokedexTextData>, ITextFile
    {
        public string? Desc { get; init; }
        public string? ShortDesc { get; init; }
        // Generation-specific properties
        public GenerationTextData? Gen3 { get; init; }
        public GenerationTextData? Gen4 { get; init; }
        public GenerationTextData? Gen5 { get; init; }
        public GenerationTextData? Gen6 { get; init; }
        public GenerationTextData? Gen7 { get; init; }
        public GenerationTextData? Gen8 { get; init; }
        public GenerationTextData? Gen9 { get; init; }
    }

    public class DefaultText : ITextFile
    {
        public string? Desc { get; init; }
        public string? ShortDesc { get; init; }
        public GenerationTextData? Gen3 { get; init; }
        public GenerationTextData? Gen4 { get; init; }
        public GenerationTextData? Gen5 { get; init; }
        public GenerationTextData? Gen6 { get; init; }
        public GenerationTextData? Gen7 { get; init; }
        public GenerationTextData? Gen8 { get; init; }
        public GenerationTextData? Gen9 { get; init; }
    }

    // Use a class instead of namespace
    public static class RandomTeamsTypes
    {
        public class TeamDetails
        {
            public int? MegaStone { get; set; }
            public int? ZMove { get; set; }
            public int? Snow { get; set; }
            public int? Hail { get; set; }
            public int? Rain { get; set; }
            public int? Sand { get; set; }
            public int? Sun { get; set; }
            public int? StealthRock { get; set; }
            public int? Spikes { get; set; }
            public int? ToxicSpikes { get; set; }
            public int? StickyWeb { get; set; }
            public int? RapidSpin { get; set; }
            public int? Defog { get; set; }
            public int? Screens { get; set; }
            public int? Illusion { get; set; }
            public int? StatusCure { get; set; }
            public int? TeraBlast { get; set; }
        }

        public class FactoryTeamDetails
        {
            public int? MegaCount { get; set; }
            public int? ZCount { get; set; }
            public int? WantsTeraCount { get; set; }
            public bool ForceResult { get; set; }
            public string? Weather { get; set; }
            public List<string>? Terrain { get; set; }
            public Dictionary<string, int> TypeCount { get; set; } = new();
            public Dictionary<string, int> TypeComboCount { get; set; } = new();
            public Dictionary<string, int> BaseFormes { get; set; } = new();
            public Dictionary<string, int> Has { get; set; } = new();
            public Dictionary<string, int> Weaknesses { get; set; } = new();
            public Dictionary<string, int> Resistances { get; set; } = new();
            public bool? Gigantamax { get; set; }
        }

        public class RandomSet
        {
            public string Name { get; set; } = string.Empty;
            public string Species { get; set; } = string.Empty;
            public string Gender { get; set; } = string.Empty;
            public bool GenderBool { get; set; }
            public List<string> Moves { get; set; } = new();
            public string Ability { get; set; } = string.Empty;
            public SparseStatsTable Evs { get; set; } = new();
            public SparseStatsTable Ivs { get; set; } = new();
            public string Item { get; set; } = string.Empty;
            public int Level { get; set; }
            public bool Shiny { get; set; }
            public string? Nature { get; set; }
            public int? Happiness { get; set; }
            public int? DynamaxLevel { get; set; }
            public bool? Gigantamax { get; set; }
            public string? TeraType { get; set; }
            public Role? PokemonRole { get; set; }
        }

        public class RandomFactorySet
        {
            public string Name { get; set; } = string.Empty;
            public string Species { get; set; } = string.Empty;
            public string Gender { get; set; } = string.Empty;
            public string Item { get; set; } = string.Empty;
            public string Ability { get; set; } = string.Empty;
            public bool Shiny { get; set; }
            public int Level { get; set; }
            public int Happiness { get; set; }
            public SparseStatsTable Evs { get; set; } = new();
            public SparseStatsTable Ivs { get; set; } = new();
            public string Nature { get; set; } = string.Empty;
            public List<string> Moves { get; set; } = new();
            public int? DynamaxLevel { get; set; }
            public bool? Gigantamax { get; set; }
            public bool? WantsTera { get; set; }
            public string? TeraType { get; set; }
        }

        public class RandomDraftFactorySet
        {
            public string Name { get; set; } = string.Empty;
            public string Species { get; set; } = string.Empty;
            public string Gender { get; set; } = string.Empty;
            public List<string> Moves { get; set; } = new();
            public string Ability { get; set; } = string.Empty;
            public SparseStatsTable Evs { get; set; } = new();
            public SparseStatsTable Ivs { get; set; } = new();
            public string Item { get; set; } = string.Empty;
            public int Level { get; set; }
            public bool Shiny { get; set; }
            public string? Nature { get; set; }
            public int? Happiness { get; set; }
            public int? DynamaxLevel { get; set; }
            public bool? Gigantamax { get; set; }
            public string? TeraType { get; set; }
            public bool? TeraCaptain { get; set; }
        }

        public class RandomSetData
        {
            public Role Role { get; set; }
            public List<string> Movepool { get; set; } = [];
            public List<string>? Abilities { get; set; }
            public List<string>? TeraTypes { get; set; }
            public List<string>? PreferredTypes { get; set; }
        }

        public class RandomSpeciesData
        {
            public int? Level { get; set; }
            public List<RandomSetData> Sets { get; set; } = [];
        }

        public enum Role
        {
            None,
            FastAttacker,
            SetupSweeper,
            Wallbreaker,
            TeraBlastUser,
            BulkyAttacker,
            BulkySetup,
            FastBulkySetup,
            BulkySupport,
            FastSupport,
            AvPivot,
            DoublesFastAttacker,
            DoublesSetupSweeper,
            DoublesWallbreaker,
            DoublesBulkyAttacker,
            DoublesBulkySetup,
            OffensiveProtect,
            BulkyProtect,
            DoublesSupport,
            ChoiceItemUser,
            ZMoveUser,
            Staller,
            Spinner,
            Generalist,
            BerrySweeper,
            ThiefUser
        }
    }

    // Mutable wrapper for immutable types
    public class Mutable<T>(T value)
        where T : class
    {
        public T Value { get; set; } = value;
    }

    // Implementations of text data interfaces for various effects
    public class AbilityTextData : IConditionTextData
    {
        public string? Desc { get; set; }
        public string? ShortDesc { get; set; }
        public string? Activate { get; set; }
        public string? AddItem { get; set; }
        public string? Block { get; set; }
        public string? Boost { get; set; }
        public string? Cant { get; set; }
        public string? ChangeAbility { get; set; }
        public string? Damage { get; set; }
        public string? End { get; set; }
        public string? Heal { get; set; }
        public string? Move { get; set; }
        public string? Start { get; set; }
        public string? Transform { get; set; }
        public string? ActivateFromItem { get; set; }
        public string? ActivateNoTarget { get; set; }
        public string? CopyBoost { get; set; }
        public string? TransformEnd { get; set; }
    }

    public class MoveTextData : IMoveTextData
    {
        public string? Desc { get; set; }
        public string? ShortDesc { get; set; }
        public string? Activate { get; set; }
        public string? AddItem { get; set; }
        public string? Block { get; set; }
        public string? Boost { get; set; }
        public string? Cant { get; set; }
        public string? ChangeAbility { get; set; }
        public string? Damage { get; set; }
        public string? End { get; set; }
        public string? Heal { get; set; }
        public string? Move { get; set; }
        public string? Start { get; set; }
        public string? Transform { get; set; }
        public string? AlreadyStarted { get; set; }
        public string? BlockSelf { get; set; }
        public string? ClearBoost { get; set; }
        public string? EndFromItem { get; set; }
        public string? Fail { get; set; }
        public string? FailSelect { get; set; }
        public string? FailTooHeavy { get; set; }
        public string? FailWrongForme { get; set; }
        public string? MegaNoItem { get; set; }
        public string? Prepare { get; set; }
        public string? RemoveItem { get; set; }
        public string? StartFromItem { get; set; }
        public string? StartFromZEffect { get; set; }
        public string? SwitchOut { get; set; }
        public string? TakeItem { get; set; }
        public string? TypeChange { get; set; }
        public string? Upkeep { get; set; }
    }

    public class ItemTextData : IConditionTextData
    {
        public string? Desc { get; set; }
        public string? ShortDesc { get; set; }
        public string? Activate { get; set; }
        public string? AddItem { get; set; }
        public string? Block { get; set; }
        public string? Boost { get; set; }
        public string? Cant { get; set; }
        public string? ChangeAbility { get; set; }
        public string? Damage { get; set; }
        public string? End { get; set; }
        public string? Heal { get; set; }
        public string? Move { get; set; }
        public string? Start { get; set; }
        public string? Transform { get; set; }
    }

    public class PokedexTextData : IBasicTextData
    {
        public string? Desc { get; set; }
        public string? ShortDesc { get; set; }
    }

    ///// <summary>
    ///// Pokemon | PokemonSwitchRequestData
    ///// </summary>
    //public interface IAnyObject
    //{
    //    //IntFalseUnion Order { get; }
    //    //int Priority { get; }
    //    //int? Speed { get; }
    //    //int SubOrder { get; }
    //    //int? EffectOrder { get; }
    //    //int? Index { get; }
    //    //EffectState AbilityState { get; }
    //    //int Count { get; }
    //    //public Dictionary<string, object> ExtraData { get; set; }
    //}

    public class AnyObject : Dictionary<string, object>
    {
        public AnyObject()
        {
        }

        public AnyObject(object value) : base(ToAnyObject(value))
        {
        }

        public AnyObject(Dictionary<string, object> dict) : base(dict)
        {
        }

        public IntFalseUnion? Order => (IntFalseUnion?)this["order"];
        public int? Priority => (int?)this["priority"];
        public int? Speed => (int?)this["speed"];
        public int? SubOrder => (int?)this["subOrder"];
        public int? EffectOrder => (int?)this["effectOrder"];
        public EffectState? AbilityState => (EffectState?)this["abilityState"];
        public int? Index => (int?)this["index"];

        public static Dictionary<string, object> ToAnyObject<T>(T? obj) where T : class
        {
            var anyObject = new Dictionary<string, object>();
            if (obj == null) return anyObject;

            Type type = obj.GetType();
            var properties = type.GetProperties(System.Reflection.BindingFlags.Public |
                                                System.Reflection.BindingFlags.Instance);

            foreach (PropertyInfo prop in properties)
            {
                object? value = prop.GetValue(obj);

                if (value == null)
                {
                    anyObject[prop.Name] = DBNull.Value;
                }
                else if (IsSimpleType(prop.PropertyType))
                {
                    anyObject[prop.Name] = value;
                }
                else if (value is System.Collections.IEnumerable enumerable and not string)
                {
                    // Handle collections (arrays, lists, etc.)
                    var list = new List<object?>();
                    foreach (object? item in enumerable)
                    {
                        if (item == null)
                            list.Add(DBNull.Value);
                        else if (IsSimpleType(item.GetType()))
                            list.Add(item);
                        else
                            list.Add(ToAnyObject(item));
                    }
                    anyObject[prop.Name] = list;
                }
                else
                {
                    // Handle nested objects
                    anyObject[prop.Name] = ToAnyObject(value);
                }
            }

            return anyObject;
        }

        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive
                   || type.IsEnum
                   || type == typeof(string)
                   || type == typeof(decimal)
                   || type == typeof(DateTime)
                   || type == typeof(Guid);
        }
    }
}
