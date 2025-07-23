using ApogeeVGC_CS.sim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC_CS.sim
{
    // Represents an ID (lowercase string, or empty)
    public class Id
    {
        public string Value { get; set; } = string.Empty;
        public bool IsId => true;
    }

    // Represents an IDEntry (lowercase string)
    public class IdEntry
    {
        public string Value { get; set; } = string.Empty;
    }

    // Represents a PokemonSlot (lowercase string, or empty)
    public class PokemonSlot
    {
        public string Value { get; set; } = string.Empty;
        public bool IsSlot => true;
    }

    // Represents a generic object with string keys and any values
    public interface IAnyObject : IDictionary<string, object> { }

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
        AG, Uber, UberAlt, OU, OUAlt, UUBL, UU, RUBL, RU, NUBL, NU, NUAlt, PUBL, PU, PUAlt, ZUBL, ZU, NFE, LC
    }

    public enum DoublesTier
    {
        DUber, DUberAlt, DOU, DOUAlt, DBL, DUU, DUUAlt, NFE, LC
    }

    public enum OtherTier
    {
        Unreleased, Illegal, CAP, CAP_NFE, CAP_LC
    }

    // Game types
    public enum GameType
    {
        Singles, Doubles, Triples, Rotation, Multi, Freeforall
    }

    public enum SideID
    {
        P1, P2, P3, P4
    }

    //// Move target types
    //public enum MoveTarget
    //{
    //    // Add specific move targets as needed
    //    Any, Normal, Self, AllAdjacent, AllAdjacentFoes, All, RandomNormal, Scripted
    //}

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

    // Mutable wrapper for immutable types
    public class Mutable<T> where T : class
    {
        public T Value { get; set; }
        public Mutable(T value) { Value = value; }
    }

    // Effect types
    public enum EffectType
    {
        Condition, Pokemon, Move, Item, Ability, Format,
        Nature, Ruleset, Weather, Status, Terrain, Rule, ValidatorRule
    }

    // Basic effect data
    public class EffectData
    {
        public string? Name { get; set; }
        public string? Desc { get; set; }
        public int? Duration { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; set; }
        public string? EffectType { get; set; }
        public bool? Infiltrates { get; set; }
        public Nonstandard? IsNonstandard { get; set; }
        public string? ShortDesc { get; set; }
    }

    public class ModdedEffectData : EffectData
    {
        public bool Inherit { get; set; }
    }

    public interface IBasicEffect
    {
        Id Id { get; set; }
        EffectType EffectType { get; set; }
        bool Exists { get; set; }
        string Fullname { get; set; }
        int Gen { get; set; }
        string SourceEffect { get; set; }
    }

    // Spread move related types
    public class SpreadMoveTargets : List<Pokemon?> { }
    public class SpreadMoveDamage : List<object> { } // number | boolean | undefined
    public class ZMoveOptions : List<ZMoveOption?> { }

    public class ZMoveOption
    {
        public string Move { get; set; } = string.Empty;
        public MoveTarget Target { get; set; }
    }

    // Battle scripts data
    public class BattleScriptsData
    {
        public int Gen { get; set; }
    }

    // Common handlers interface
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

    // Text data interfaces
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

    // Text file wrapper
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

    // Specific text types
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
        public bool TryGetValue(string key, out object value) => _data.TryGetValue(key, out value);
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _data.GetEnumerator();
    }

    // Using aliases for text types
    public class AbilityText : TextFile<AbilityTextData> { }
    public class MoveText : TextFile<MoveTextData> { }
    public class ItemText : TextFile<ItemTextData> { }
    public class PokedexText : TextFile<PokedexTextData> { }
    public class DefaultText : DefaultTextData { }

    // Player options
    public class PlayerOptions
    {
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public int? Rating { get; set; }
        public List<PokemonSet>? Team { get; set; }
        public string? TeamString { get; set; }
        public PRNGSeed? Seed { get; set; }
    }

    // Random teams types namespace
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

    // type Effect = Ability | Item | ActiveMove | Species | Condition | Format;

    // Forward declarations for interfaces (to be implemented by specific classes)
    public interface IEffect { }
    public interface IAbility : IEffect { }
    public interface IItem : IEffect { }
    public interface IActiveMove : IEffect { }
    public interface ISpecies : IEffect { }
    public interface ICondition : IEffect { }
    public interface IFormat : IEffect { }
    public interface IPokemonSet { }
    public interface IPRNGSeed { }
    public interface ITypeInfo { }


    // Forward declarations for the main simulation classes
    // public class Battle { }
    // public class Pokemon { }
    //public class Side { }
    //public class Field { }
    public class Effect { }
    public class Ability { }
    // public class Item { }
    //public class ActiveMove { }
    // public class Species { }
    // public class Condition { }
    // public class Format { }
    // public class Nature { }
    // public class Move { }
    //public class PokemonSet { }
    //public class PRNGSeed { }
    //public class PRNG { }
    // public class ModdedDex { }
    // public class Choice { }
}