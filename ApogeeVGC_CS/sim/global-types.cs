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

        public static bool IsValid(string id)
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
    public class IdEntry : Id
    {
        public override bool IsId => false;
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

    public class StatsExceptHpTable : Dictionary<StatIdExceptHp, int>;

    public class StatsTable : Dictionary<StatId, int>
    {
        public StatsTable()
        {
            this[StatId.Hp] = 0; // Initialize HP to 0
        }
        public StatsTable(StatsTable other) : this()
        {
            foreach (var kvp in other)
            {
                this[kvp.Key] = kvp.Value;
            }
        }
    }

    public class SparseStatsTable : Dictionary<StatId, int>;

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

    // Tier types from TierTypes namespace
    public enum SinglesTier
    {
        Ag, Uber, UberAlt, Ou, OuAlt, Uubl, Uu, Rubl, Ru, Nubl, Nu, NuAlt, Publ, Pu,
        PuAlt, Zubl, Zu, Nfe, Lc
    }

    public enum DoublesTier
    {
        DUber, DUberAlt, Dou, DouAlt, Dbl, Duu, DuuAlt, Nfe, Lc
    }

    public enum OtherTier
    {
        Unreleased, Illegal, Cap, CapNfe, CapLc
    }

    public class EventInfo
    {
        public int Generation { get; set; }
        public int? Level { get; set; }
        public bool? Shiny { get; set; } // true: always shiny, 1: sometimes shiny, false/null: never shiny
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
    public interface IEffect;

    public interface ICommonHandlers
    {
        Func<Battle, int, Pokemon, Pokemon, IEffect, int> ModifierEffect { get; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int> ModifierMove { get; }
        Func<Battle, Pokemon, Pokemon, IActiveMove, bool?> ResultMove { get; }
        Func<Battle, Pokemon, Pokemon, IActiveMove, object> ExtResultMove { get; }
        Action<Battle, Pokemon, Pokemon, IEffect> VoidEffect { get; }
        Action<Battle, Pokemon, Pokemon, IActiveMove> VoidMove { get; }
        Func<Battle, int, Pokemon, Pokemon, IEffect, int> ModifierSourceEffect { get; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int> ModifierSourceMove { get; }
        Func<Battle, Pokemon, Pokemon, IActiveMove, bool?> ResultSourceMove { get; }
        Func<Battle, Pokemon, Pokemon, IActiveMove, object> ExtResultSourceMove { get; }
        Action<Battle, Pokemon, Pokemon, IEffect> VoidSourceEffect { get; }
        Action<Battle, Pokemon, Pokemon, IActiveMove> VoidSourceMove { get; }
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
        Nature, Ruleset, Weather, Status, Terrain, Rule, ValidatorRule
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
        P1, P2, P3, P4
    }

    public class SpreadMoveTargets : List<Pokemon?>;
    public class SpreadMoveDamage : List<object>; // number | boolean | undefined
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
        Func<Pokemon, StatIdExceptHp, int, int?, int>? CalculateStat { get; set; }
        Func<Pokemon, bool?, bool>? CureStatus { get; set; }
        Func<Pokemon, object, object?, object?, int>? DeductPp { get; set; }
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
        Func<Pokemon, StatIdExceptHp, bool?, bool?, bool?, int>? GetStat { get; set; }
        Func<Pokemon, bool?, bool?, List<string>>? GetTypes { get; set; }
        Func<Pokemon, int>? GetWeight { get; set; }
        Func<Pokemon, object, bool>? HasAbility { get; set; }
        Func<Pokemon, object, bool>? HasItem { get; set; }
        Func<Pokemon, bool?, bool?>? IsGrounded { get; set; }
        Action<Pokemon, StatIdExceptHp, int>? ModifyStat { get; set; }
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
        Action<Battle, object>? GetActionSpeed { get; set; }
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
        public PrngSeed? Seed { get; set; }
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
    public class AbilityText : TextFile<AbilityTextData>;
    public class MoveText : TextFile<MoveTextData>;
    public class ItemText : TextFile<ItemTextData>;
    public class PokedexText : TextFile<PokedexTextData>;
    public class DefaultText; //: DefaultTextData { }

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
}
