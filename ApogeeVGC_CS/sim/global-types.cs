using ApogeeVGC_CS.sim;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC_CS.sim
{
    // An ID must have only lowercase alphanumeric characters
    public class Id
    {
        private string _value = string.Empty;
        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    if (IsValid(value))
                    {
                        _value = value;
                    }
                    else
                    {
                        throw new ArgumentException("Invalid ID format. Must be lowercase alphanumeric.");
                    }
                }
            }
        }
        public bool IsId => true;
        public bool IsEmpty => string.IsNullOrEmpty(Value);

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

        /**
         * Returns true if the sequence of elements of searchString converted to a String is the
         * same as the corresponding elements of this object (converted to a String) starting at
         * endPosition – length(this). Otherwise returns false.
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
            var type = obj.GetType();
            var idProp = type.GetProperty("id") ?? type.GetProperty("userId") ?? type.GetProperty("roomId");

            if (idProp != null && idProp.GetValue(obj) is string propValue)
            {
                return FromString(propValue);
            }
            else
            {
                return string.Empty; // Return empty string if no valid ID found
            }
        }

        private static bool IsValid(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            foreach (char c in id)
            {
                if (!char.IsLower(c) && !char.IsDigit(c)) return false;
            }
            return true;
        }
    }

    // must be lowercase alphanumeric
    public class IdEntry
    {
        private string _value = string.Empty;
        public string Value
        {
            get => _value;
            set => _value = value?.ToLowerInvariant() ?? string.Empty;
        }
    }

    // must be lowercase alphanumeric
    public class PokemonSlot
    {
        private string _value = string.Empty;
        public string Value
        {
            get => _value;
            set => _value = value?.ToLowerInvariant() ?? string.Empty;
        }
        public bool IsSlot => true;
    }

    // Represents a generic object with string keys and any values
    public interface IAnyObject : IDictionary<string, object>
    {
        public bool TryGetInt(string key, [MaybeNullWhen(false)] out int @int);
        public bool TryGetBool(string key, [MaybeNullWhen(false)] out bool @bool);
        public bool TryGetString(string key, [MaybeNullWhen(false)] out string @string);
        public bool TryGetId(string key, [MaybeNullWhen(false)] out Id id);
        public bool TryGetEnum<TEnum>(string key, [MaybeNullWhen(false)] out TEnum enumValue) where TEnum : Enum;
        public bool TryGetStruct<TStruct>(string key, [MaybeNullWhen(false)] out TStruct structValue) where TStruct : struct;
        public bool TryGetClass<TClass>(string key, [MaybeNullWhen(false)] out TClass? classValue) where TClass : class;
        public bool TryGetObject(string key, [MaybeNullWhen(false)] out object? obj);
        public bool TryGetList<T>(string key, [MaybeNullWhen(false)] out List<T> list) where T : class;
        public bool TryGetDictionary<TKey, TValue>(string key, [MaybeNullWhen(false)] out Dictionary<TKey, TValue> dict) where TKey : notnull;
        public bool TryGetNullable<T>(string key, out T? value) where T : struct;
        public bool TryGetFunction<T>(string key, [MaybeNullWhen(false)] out Func<T> function);
        public bool TryGetAction<T>(string key, [MaybeNullWhen(false)] out Action<T> action);
    }

    public enum GenderName
    {
        M, F, N, Empty
    }

    public enum StatIDExceptHP
    {
        Atk, Def, Spa, Spd, Spe
    }

    public enum StatID
    {
        Hp, Atk, Def, Spa, Spd, Spe
    }

    public class StatsExceptHPTable : Dictionary<StatIDExceptHP, int> { }
    public class StatsTable : Dictionary<StatID, int> { }
    public class SparseStatsTable : Dictionary<StatID, int> { }

    public enum BoostID
    {
        Atk, Def, Spa, Spd, Spe, Accuracy, Evasion
    }

    public class BoostsTable : Dictionary<BoostID, int> { }
    public class SparseBoostsTable : Dictionary<BoostID, int> { }

    public enum Nonstandard
    {
        Past, Future, Unobtainable, CAP, LGPE, Custom, Gigantamax
    }

    // Tier types from TierTypes namespace
    public enum SinglesTier
    {
        AG, Uber, UberAlt, OU, OUAlt, UUBL, UU, RUBL, RU, NUBL, NU, NUAlt, PUBL, PU,
        PUAlt, ZUBL, ZU, NFE, LC
    }

    public enum DoublesTier
    {
        DUber, DUberAlt, DOU, DOUAlt, DBL, DUU, DUUAlt, NFE, LC
    }

    public enum OtherTier
    {
        Unreleased, Illegal, CAP, CAP_NFE, CAP_LC
    }

    public class EventInfo
    {
        public int Generation { get; set; }
        public int? Level { get; set; }
        // true: always shiny, 1: sometimes shiny, false/null: never shiny
        public bool? Shiny { get; set; }
        public int? ShinySometimes { get; set; } // Use this if you need to distinguish '1'
        public GenderName? Gender { get; set; }
        public string? Nature { get; set; }
        public SparseStatsTable? IVs { get; set; }
        public int? PerfectIVs { get; set; }
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
    /// Base interface for all effect types.
    /// </summary>
    public interface IEffect { }

    /// <summary>
    /// Represents an ability effect.
    /// </summary>
    public interface IAbility : IEffect { }

    /// <summary>
    /// Represents an item effect.
    /// </summary>
    public interface IItem : IEffect { }

    /// <summary>
    /// Represents an active move effect.
    /// </summary>
    public interface IActiveMove : IEffect { }

    /// <summary>
    /// Represents a species effect.
    /// </summary>
    public interface ISpecies : IEffect { }

    /// <summary>
    /// Represents a condition effect.
    /// </summary>
    public interface ICondition : IEffect { }

    /// <summary>
    /// Represents a format effect.
    /// </summary>
    public interface IFormat : IEffect { }

    public interface ICommonHandlers
    {
        Func<Battle, int, Pokemon, Pokemon, IEffect, int> ModifierEffect { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int> ModifierMove { get; set; }
        Func<Battle, Pokemon, Pokemon, IActiveMove, bool?> ResultMove { get; set; }
        Func<Battle, Pokemon, Pokemon, IActiveMove, object> ExtResultMove { get; set; }
        Action<Battle, Pokemon, Pokemon, IEffect> VoidEffect { get; set; }
        Action<Battle, Pokemon, Pokemon, IActiveMove> VoidMove { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IEffect, int> ModifierSourceEffect { get; set; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int> ModifierSourceMove { get; set; }
        Func<Battle, Pokemon, Pokemon, IActiveMove, bool?> ResultSourceMove { get; set; }
        Func<Battle, Pokemon, Pokemon, IActiveMove, object> ExtResultSourceMove { get; set; }
        Action<Battle, Pokemon, Pokemon, IEffect> VoidSourceEffect { get; set; }
        Action<Battle, Pokemon, Pokemon, IActiveMove> VoidSourceMove { get; set; }
    }

    public interface IEffectData
    {
        public string? Name { get; set; }
        public string? Desc { get; set; }
        public int? Duration { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; set; }
        public string? EffectTypeString { get; set; }
        public bool? Infiltrates { get; set; }
        public Nonstandard? IsNonstandard { get; set; }
        public string? ShortDesc { get; set; }
    }

    //public class ModdedEffectData : IEffectData
    //{
    //    public bool Inherit { get; set; } = true;
    //    public string? Name { get; set; } = string.Empty;
    //    public string? Desc { get; set; } = string.Empty;
    //    public int? Duration { get; set; } = null;
    //    public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; set; } = null;
    //    public string? EffectTypeString { get; set; } = null;
    //    public bool? Infiltrates { get; set; } = null;
    //    public Nonstandard? IsNonstandard { get; set; } = null;
    //    public string? ShortDesc { get; set; } = string.Empty;
    //}

    public interface IModdedEffectData : IEffectData
    {
        public bool Inherit { get; set; }
    }

    public enum EffectType
    {
        Condition, Pokemon, Move, Item, Ability, Format,
        Nature, Ruleset, Weather, Status, Terrain, Rule, ValidatorRule
    }

    public interface IBasicEffect : IEffectData
    {
        Id Id { get; set; }
        EffectType EffectType { get; set; }
        bool Exists { get; set; }
        string Fullname { get; set; }
        int Gen { get; set; }
        string SourceEffect { get; set; }
    }

    public enum GameType
    {
        Singles, Doubles, Triples, Rotation, Multi, FreeForAll
    }

    public enum SideID
    {
        P1, P2, P3, P4
    }

    public class SpreadMoveTargets : List<Pokemon?> { }
    public class SpreadMoveDamage : List<object> { } // number | boolean | undefined
    public class ZMoveOptions : List<ZMoveOption?> { }

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

    public interface IModdedBattleActions
    {
        bool? Inherit { get; set; }
        Action<BattleActions, List<Pokemon>, Pokemon, ActiveMove>? AfterMoveSecondaryEvent { get; set; }
        Func<BattleActions, int, Move, Pokemon, int>? CalcRecoilDamage { get; set; }
        Func<BattleActions, Pokemon, string?>? CanMegaEvo { get; set; }
        Func<BattleActions, Pokemon, string?>? CanMegaEvoX { get; set; }
        Func<BattleActions, Pokemon, string?>? CanMegaEvoY { get; set; }
        Func<BattleActions, Pokemon, string?>? CanTerastallize { get; set; }
        Func<BattleActions, Pokemon, string?>? CanUltraBurst { get; set; }
        Func<BattleActions, Pokemon, ZMoveOptions?>? CanZMove { get; set; }
        Func<BattleActions, SpreadMoveDamage, SpreadMoveTargets, Pokemon, ActiveMove, ActiveMove, bool?, bool?, SpreadMoveDamage>? ForceSwitch { get; set; }
        Func<BattleActions, Move, Pokemon, ActiveMove>? GetActiveMaxMove { get; set; }
        Func<BattleActions, Move, Pokemon, ActiveMove>? GetActiveZMove { get; set; }
        Func<BattleActions, Move, Pokemon, Move?>? GetMaxMove { get; set; }
        Func<BattleActions, SpreadMoveDamage, SpreadMoveTargets, Pokemon, ActiveMove, ActiveMove, bool?, bool?, SpreadMoveDamage>? GetSpreadDamage { get; set; }
        Func<BattleActions, Move, Pokemon, bool, string?>? GetZMove { get; set; }
        Func<BattleActions, List<Pokemon>, Pokemon, ActiveMove, List<bool>>? HitStepAccuracy { get; set; }
        Action<BattleActions, List<Pokemon>, Pokemon, ActiveMove>? HitStepBreakProtect { get; set; }
        Func<BattleActions, List<Pokemon>, Pokemon, ActiveMove, SpreadMoveDamage>? HitStepMoveHitLoop { get; set; }
        Func<BattleActions, List<Pokemon>, Pokemon, ActiveMove, List<bool>>? HitStepTryImmunity { get; set; }
        Action<BattleActions, List<Pokemon>, Pokemon, ActiveMove>? HitStepStealBoosts { get; set; }
        Func<BattleActions, List<Pokemon>, Pokemon, ActiveMove, List<object>>? HitStepTryHitEvent { get; set; }
        Func<BattleActions, List<Pokemon>, Pokemon, ActiveMove, List<bool>>? HitStepInvulnerabilityEvent { get; set; }
        Func<BattleActions, List<Pokemon>, Pokemon, ActiveMove, List<bool>>? HitStepTypeImmunity { get; set; }
        Func<BattleActions, Pokemon?, Pokemon, ActiveMove, ActiveMove?, bool?, bool?, object>? MoveHit { get; set; }
        Action<BattleActions, object>? RunAction { get; set; }
        Func<BattleActions, Pokemon, bool>? RunMegaEvo { get; set; }
        Func<BattleActions, Pokemon, bool>? RunMegaEvoX { get; set; }
        Func<BattleActions, Pokemon, bool>? RunMegaEvoY { get; set; }
        Action<BattleActions, object, Pokemon, int, object>? RunMove { get; set; }
        Func<BattleActions, SpreadMoveDamage, SpreadMoveTargets, Pokemon, ActiveMove, ActiveMove, bool?, bool?, SpreadMoveDamage>? RunMoveEffects { get; set; }
        Func<BattleActions, Pokemon, bool>? RunSwitch { get; set; }
        Action<BattleActions, ActiveMove, Pokemon>? RunZPower { get; set; }
        Action<BattleActions, SpreadMoveTargets, Pokemon, ActiveMove, ActiveMove, bool?>? Secondaries { get; set; }
        Action<BattleActions, SpreadMoveTargets, Pokemon, ActiveMove, ActiveMove, bool?, bool?>? SelfDrops { get; set; }
        Func<BattleActions, SpreadMoveTargets, Pokemon, ActiveMove, ActiveMove?, bool?, bool?, Tuple<SpreadMoveDamage, SpreadMoveTargets>>? SpreadMoveHit { get; set; }
        Func<BattleActions, Pokemon, int, object, bool?, object>? SwitchIn { get; set; }
        Func<BattleActions, string, bool>? TargetTypeChoices { get; set; }
        Action<BattleActions, Pokemon>? Terastallize { get; set; }
        Func<BattleActions, Pokemon, Pokemon, ActiveMove, object>? TryMoveHit { get; set; }
        Func<BattleActions, SpreadMoveDamage, SpreadMoveTargets, Pokemon, ActiveMove, ActiveMove, bool?, SpreadMoveDamage>? TryPrimaryHitEvent { get; set; }
        Func<BattleActions, List<Pokemon>, Pokemon, ActiveMove, bool?, bool>? TrySpreadMoveHit { get; set; }
        Func<BattleActions, Move, Pokemon, object?, bool>? UseMove { get; set; }
        Func<BattleActions, Move, Pokemon, object?, bool>? UseMoveInner { get; set; }
        Func<BattleActions, Pokemon, Pokemon, object, bool, object>? GetDamage { get; set; }
        Action<BattleActions, int, Pokemon, Pokemon, ActiveMove, bool?>? ModifyDamage { get; set; }

        // OMs (Other Metagames)
        Func<BattleActions, Species, object, Species>? MutateOriginalSpecies { get; set; }
        Func<BattleActions, Species, Pokemon?, object>? GetFormeChangeDeltas { get; set; }
        Func<BattleActions, string, string, Pokemon?, Species>? GetMixedSpecies { get; set; }
    }

    public interface IModdedBattleSide
    {
        bool? Inherit { get; set; }
        Func<Side, object, object, object, object, bool>? AddSideCondition { get; set; }
        Func<Side, bool?, List<Pokemon>>? Allies { get; set; }
        Func<Side, bool>? CanDynamaxNow { get; set; }
        Func<Side, string?, object>? ChooseSwitch { get; set; }
        Func<Side, string>? GetChoice { get; set; }
        Func<Side, bool?, object>? GetRequestData { get; set; }
    }

    public interface IModdedBattlePokemon
    {
        bool? Inherit { get; set; }
        object? LostItemForDelibird { get; set; }
        Func<Pokemon, SparseBoostsTable, object>? BoostBy { get; set; }
        Action<Pokemon>? ClearBoosts { get; set; }
        Func<Pokemon, StatIDExceptHP, int, int?, int>? CalculateStat { get; set; }
        Func<Pokemon, bool?, bool>? CureStatus { get; set; }
        Func<Pokemon, object, object?, object?, int>? DeductPP { get; set; }
        Func<Pokemon, bool?, Pokemon?, object?, bool>? EatItem { get; set; }
        Func<Pokemon, Id>? EffectiveWeather { get; set; }
        Func<Pokemon, object, object, object?, string?, bool>? FormeChange { get; set; }
        Func<Pokemon, object, bool>? HasType { get; set; }
        Func<Pokemon, object>? GetAbility { get; set; }
        Func<Pokemon, int>? GetActionSpeed { get; set; }
        Func<Pokemon, object>? GetItem { get; set; }
        Func<Pokemon, object>? GetMoveRequestData { get; set; }
        Func<Pokemon, string?, bool?, List<object>>? GetMoves { get; set; }
        Func<Pokemon, ActiveMove, Pokemon, object>? GetMoveTargets { get; set; }
        Func<Pokemon, StatIDExceptHP, bool?, bool?, bool?, int>? GetStat { get; set; }
        Func<Pokemon, bool?, bool?, List<string>>? GetTypes { get; set; }
        Func<Pokemon, int>? GetWeight { get; set; }
        Func<Pokemon, object, bool>? HasAbility { get; set; }
        Func<Pokemon, object, bool>? HasItem { get; set; }
        Func<Pokemon, bool?, bool?>? IsGrounded { get; set; }
        Action<Pokemon, StatIDExceptHP, int>? ModifyStat { get; set; }
        Action<Pokemon, ActiveMove, int?>? MoveUsed { get; set; }
        Action<Pokemon>? RecalculateStats { get; set; }
        Func<Pokemon, ActiveMove, int>? RunEffectiveness { get; set; }
        Func<Pokemon, object, object?, bool>? RunImmunity { get; set; }
        Func<Pokemon, object, Pokemon?, bool, object>? SetAbility { get; set; }
        Func<Pokemon, object, Pokemon?, object?, bool>? SetItem { get; set; }
        Func<Pokemon, object, Pokemon?, object?, bool, bool>? SetStatus { get; set; }
        Func<Pokemon, Pokemon?, object>? TakeItem { get; set; }
        Func<Pokemon, Pokemon, object?, bool>? TransformInto { get; set; }
        Func<Pokemon, Pokemon?, object?, bool>? UseItem { get; set; }
        Func<Pokemon, bool>? IgnoringAbility { get; set; }
        Func<Pokemon, bool>? IgnoringItem { get; set; }
        // OMs (Other Metagames)
        Func<Pokemon, bool?, List<string>>? GetLinkedMoves { get; set; }
        Func<Pokemon, string, bool>? HasLinkedMove { get; set; }
    }

    public interface IModdedBattleQueue
    {
        bool? Inherit { get; set; }
        Func<BattleQueue, ActionChoice, bool?, List<Action>>? ResolveAction { get; set; }
    }

    public interface IModdedField
    {
        bool? Inherit { get; set; }
        Func<Field, bool>? SuppressingWeather { get; set; }
        Func<Field, object, object, object?, bool>? AddPseudoWeather { get; set; }
        Func<Field, object, object, object?, bool?>? SetWeather { get; set; }
        Func<Field, object, object, object?, bool>? SetTerrain { get; set; }
    }

    public interface IModdedBattleScriptsData
    {
        string? Inherit { get; set; }
        IModdedBattleActions? Actions { get; set; }
        IModdedBattlePokemon? Pokemon { get; set; }
        IModdedBattleQueue? Queue { get; set; }
        IModdedField? Field { get; set; }
        IModdedBattleSide? Side { get; set; }
        Func<Battle, SparseBoostsTable, Pokemon, Pokemon?, object?, bool?, bool?, object>? Boost { get; set; }
        Action<Battle, string>? Debug { get; set; }
        Action<Battle, IAnyObject>? GetActionSpeed { get; set; }
        Action<ModdedDex>? Init { get; set; }
        Func<Battle, bool[], string[], bool?>? MaybeTriggerEndlessBattleClause { get; set; }
        Func<Battle, StatsTable, PokemonSet, StatsTable>? NatureModify { get; set; }
        Action<Battle>? EndTurn { get; set; }
        Action<Battle, object>? RunAction { get; set; }
        Func<Battle, StatsTable, PokemonSet, StatsTable>? SpreadModify { get; set; }
        Action<Battle>? Start { get; set; }
        Func<Battle, bool>? SuppressingWeather { get; set; }
        Func<double, double>? Trunc { get; set; }
        Func<Battle, object?, bool>? Win { get; set; }
        Func<Battle, bool?, bool?, bool?, bool?>? FaintMessages { get; set; }
        Func<Battle, bool>? Tiebreak { get; set; }
        Func<Battle, ActiveMove, Pokemon, Pokemon, bool?, bool>? CheckMoveMakesContact { get; set; }
        Func<Battle, object?, bool?>? CheckWin { get; set; }
        Action<Battle, string, List<Pokemon>?>? FieldEvent { get; set; }
        Func<Battle, bool?, bool?, List<Pokemon>>? GetAllActive { get; set; }
    }

    public class PlayerOptions
    {
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public int? Rating { get; set; }
        public List<PokemonSet>? Team { get; set; }
        public string? TeamString { get; set; }
        public PRNGSeed? Seed { get; set; }
    }
    public interface IBasicTextData
    {
        string? Desc { get; set; }
        string? ShortDesc { get; set; }
    }

    public interface IConditionTextData : IBasicTextData
    {
        string? Activate { get; set; }
        string? AddItem { get; set; }
        string? Block { get; set; }
        string? Boost { get; set; }
        string? Cant { get; set; }
        string? ChangeAbility { get; set; }
        string? Damage { get; set; }
        string? End { get; set; }
        string? Heal { get; set; }
        string? Move { get; set; }
        string? Start { get; set; }
        string? Transform { get; set; }
    }

    public interface IMoveTextData : IConditionTextData
    {
        string? AlreadyStarted { get; set; }
        string? BlockSelf { get; set; }
        string? ClearBoost { get; set; }
        string? EndFromItem { get; set; }
        string? Fail { get; set; }
        string? FailSelect { get; set; }
        string? FailTooHeavy { get; set; }
        string? FailWrongForme { get; set; }
        string? MegaNoItem { get; set; }
        string? Prepare { get; set; }
        string? RemoveItem { get; set; }
        string? StartFromItem { get; set; }
        string? StartFromZEffect { get; set; }
        string? SwitchOut { get; set; }
        string? TakeItem { get; set; }
        string? TypeChange { get; set; }
        string? Upkeep { get; set; }
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
    public class AbilityText : TextFile<AbilityTextData> { }
    public class MoveText : TextFile<MoveTextData> { }
    public class ItemText : TextFile<ItemTextData> { }
    public class PokedexText : TextFile<PokedexTextData> { }
    public class DefaultText : DefaultTextData { }

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
            public Role? Role { get; set; }
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
            public List<string> Movepool { get; set; } = new();
            public List<string>? Abilities { get; set; }
            public List<string>? TeraTypes { get; set; }
            public List<string>? PreferredTypes { get; set; }
        }

        public class RandomSpeciesData
        {
            public int? Level { get; set; }
            public List<RandomSetData> Sets { get; set; } = new();
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
            AVPivot,
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
    public class Mutable<T> where T : class
    {
        public T Value { get; set; }
        public Mutable(T value) { Value = value; }
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

    public class DefaultTextData : IAnyObject
    {
        private readonly Dictionary<string, object> _data = new();

        public object this[string key]
        {
            get => _data[key];
            set => _data[key] = value;
        }

        public ICollection<string> Keys => _data.Keys;
        public ICollection<object> Values => _data.Values;
        public int Count => _data.Count;
        public bool IsReadOnly => false;

        public void Add(string key, object value) => _data.Add(key, value);
        public void Add(KeyValuePair<string, object> item) => _data.Add(item.Key, item.Value);
        public void Clear() => _data.Clear();
        public bool Contains(KeyValuePair<string, object> item) => _data.Contains(item);
        public bool ContainsKey(string key) => _data.ContainsKey(key);
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => ((IDictionary<string, object>)_data).CopyTo(array, arrayIndex);
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _data.GetEnumerator();
        public bool Remove(string key) => _data.Remove(key);
        public bool Remove(KeyValuePair<string, object> item) => _data.Remove(item.Key);

        public bool TryGetBool(string key, [MaybeNullWhen(false)] out bool @bool)
        {
            if (_data.TryGetValue(key, out var value) && value is bool b)
            {
                @bool = b;
                return true;
            }
            @bool = default;
            return false;
        }

        public bool TryGetEffectType(string key, [MaybeNullWhen(false)] out EffectType effectType)
        {
            if (_data.TryGetValue(key, out var value) && value is EffectType et)
            {
                effectType = et;
                return true;
            }
            effectType = default;
            return false;
        }

        public bool TryGetId(string key, [MaybeNullWhen(false)] out Id id)
        {
            if (_data.TryGetValue(key, out var value) && value is Id i)
            {
                id = i;
                return true;
            }
            id = default;
            return false;
        }

        public bool TryGetInt(string key, [MaybeNullWhen(false)] out int @int)
        {
            if (_data.TryGetValue(key, out var value) && value is int i)
            {
                @int = i;
                return true;
            }
            @int = default;
            return false;
        }

        public bool TryGetString(string key, [MaybeNullWhen(false)] out string @string)
        {
            if (_data.TryGetValue(key, out var value) && value is string s)
            {
                @string = s;
                return true;
            }
            @string = default;
            return false;
        }

        public bool TryGetEnum<TEnum>(string key, [MaybeNullWhen(false)] out TEnum @enum) where TEnum : Enum
        {
            if (_data.TryGetValue(key, out var value) && value is TEnum e)
            {
                @enum = e;
                return true;
            }
            @enum = default;
            return false;
        }

        // for IDictionary
        public bool TryGetValue(string key, out object value) => _data.TryGetValue(key, out value);
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _data.GetEnumerator();

        public bool TryGetStruct<TStruct>(string key, [MaybeNullWhen(false)] out TStruct structValue) where TStruct : struct
        {
            if (_data.TryGetValue(key, out var value) && value is TStruct s)
            {
                structValue = s;
                return true;
            }
            structValue = default;
            return false;
        }

        public bool TryGetClass<TClass>(string key, [MaybeNullWhen(false)] out TClass? classValue) where TClass : class
        {
            if (_data.TryGetValue(key, out var value) && value is TClass c)
            {
                classValue = c;
                return true;
            }
            classValue = default;
            return false;
        }

        public bool TryGetObject(string key, [MaybeNullWhen(false)] out object? obj)
        {
            if (_data.TryGetValue(key, out var value))
            {
                obj = value;
                return true;
            }
            obj = default;
            return false;
        }

        public bool TryGetList<T>(string key, [MaybeNullWhen(false)] out List<T> list) where T : class
        {
            if (_data.TryGetValue(key, out var value) && value is List<T> l)
            {
                list = l;
                return true;
            }
            list = default;
            return false;
        }

        public bool TryGetDictionary<TKey, TValue>(string key, [MaybeNullWhen(false)] out Dictionary<TKey, TValue> dict) where TKey : notnull
        {
            if (_data.TryGetValue(key, out var value) && value is Dictionary<TKey, TValue> d)
            {
                dict = d;
                return true;
            }
            dict = default;
            return false;
        }

        public bool TryGetNullable<T>(string key, [MaybeNullWhen(false)] out T? value) where T : struct
        {
            if (_data.TryGetValue(key, out var obj) && obj is T v)
            {
                value = v;
                return true;
            }
            value = default;
            return false;
        }

        public bool TryGetFunction<T>(string key, [MaybeNullWhen(false)] out Func<T> function)
        {
            if (_data.TryGetValue(key, out var value) && value is Func<T> func)
            {
                function = func;
                return true;
            }
            function = default;
            return false;
        }

        public bool TryGetAction<T>(string key, [MaybeNullWhen(false)] out Action<T> action)
        {
            if (_data.TryGetValue(key, out var value) && value is Action<T> act)
            {
                action = act;
                return true;
            }
            action = default;
            return false;
        }
    }

    public class AnyObjectEmpty : DefaultTextData, IAnyObject
    {
    }


    //public interface IPokemonSet { }
    //public interface IPRNGSeed { }
    //public interface ITypeInfo { }
}
