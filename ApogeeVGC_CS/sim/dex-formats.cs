using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using static ApogeeVGC_CS.sim.Format;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC_CS.sim
{
    public interface IFormatData : IEventMethods, IFormat, IFormatListEntry { }

    // Represents the element of a "FormatList" type
    public interface IFormatListEntry { }

    public class SectionEntry : IFormatListEntry
    {
        public string Section { get; set; } = string.Empty;
        public int? Column { get; set; } = null;
    }

    // export type FormatList = (FormatData | { section: string, column?: number })[];
    public class FormatList : List<IFormatListEntry> { }

    // export type ModdedFormatData = FormatData | Omit<FormatData, 'name'> & { inherit: true };
    public interface IModdedFormatData : IFormatData
    {
        bool Inherit { get; set; }
    }

    // export interface FormatDataTable { [id: IDEntry]: FormatData }
    public class FormatDataTable : Dictionary<IdEntry, IFormatData> { }

    // export interface ModdedFormatDataTable { [id: IDEntry]: ModdedFormatData }
    public class ModdedFormatDataTable : Dictionary<IdEntry, IModdedFormatData> { }

    public enum FormatEffectType
    {
        Format,
        Ruleset,
        Rule,
        ValidatorRule
    }

    // export type ComplexBan = [string, string, number, string[]];
    public class ComplexBan
    {
        public string Rule { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public int Limit { get; set; } = 0;
        public List<string> Bans { get; set; } = [];
    }

    public interface IGameTimerSettings
    {
        public bool DcTimer { get; set; }
        public bool DcTimerBank { get; set; }
        public int Starting { get; set; }
        public int Grace { get; set; }
        public int AddPerTurn { get; set; }
        public int MaxPerTurn { get; set; }
        public int MaxFirstTurn { get; set; }
        public bool TimeoutAutoChoose { get; set; }
        public bool Accelerate { get; set; }
    }

    /**
     * A RuleTable keeps track of the rules that a format has. The key can be:
     * - '[ruleid]' the ID of a rule in effect
     * - '-[thing]' or '-[category]:[thing]' ban a thing
     * - '+[thing]' or '+[category]:[thing]' allow a thing (override a ban)
     * [category] is one of: item, move, ability, species, basespecies
     *
     * The value is the name of the parent rule (blank for the active format).
     */
    public class RuleTable : Dictionary<string, string>
    {
        public List<ComplexBan> ComplexBans { get; set; } = [];
        public List<ComplexBan> ComplexTeamBans { get; set; } = [];
        public (Func<object, bool> CheckCanLearn, string)? CheckCanLearn { get; set; } = null;
        public (IGameTimerSettings Timer, string)? Timer { get; set; } = null;
        public List<string> TagRules { get; set; } = [];
        public Dictionary<string, string> ValueRules { get; set; } = [];

        public int MinTeamSize { get; set; }
        public int MaxTeamSize { get; set; }
        public int? PickedTeamSize { get; set; }
        public int? MaxTotalLevel { get; set; }
        public int MaxMoveCount { get; set; }
        public int MinSourceGen { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public int DefaultLevel { get; set; }
        public int? AdjustLevel { get; set; }
        public int? AdjustLevelDown { get; set; }
        public int? EvLimit { get; set; }

        //public RuleTable()
        //{
        //}

        public bool IsBanned(string thing)
        {
            throw new NotImplementedException("IsBanned method is not implemented yet.");
        }

        public bool IsBannedSpecies(string species)
        {
            throw new NotImplementedException("IsBannedSpecies method is not implemented yet.");
        }

        public bool IsRestricted(string thing)
        {
            throw new NotImplementedException("IsRestricted method is not implemented yet.");
        }

        public bool IsRestrictedSpecies(string species)
        {
            throw new NotImplementedException("IsRestrictedSpecies method is not implemented yet.");
        }

        public List<string> GetTagRules()
        {
            throw new NotImplementedException("GetTagRules method is not implemented yet.");
        }

        public string? Check(string thing, Dictionary<string, bool>? setHas = null)
        {
            throw new NotImplementedException("Check method is not implemented yet.");
        }

        public string? GetReason(string key)
        {
            throw new NotImplementedException("GetReason method is not implemented yet.");
        }

        public string Blame(string key)
        {
            throw new NotImplementedException("Blame method is not implemented yet.");
        }

        public int GetComplexBanIndex(List<ComplexBan> complexBans, string rule)
        {
            throw new NotImplementedException("GetComplexBanIndex method is not implemented yet.");
        }

        public void AddComplexBan(string rule, string source, int limit, List<string> bans)
        {
            throw new NotImplementedException("AddComplexBan method is not implemented yet.");
        }

        public void AddComplexTeamBan(string rule, string source, int limit, List<string> bans)
        {
            throw new NotImplementedException("AddComplexTeamBan method is not implemented yet.");
        }

        public void ResolveNumbers(Format format, DexFormats dex)
        {
            throw new NotImplementedException("ResolveNumbers method is not implemented yet.");
        }

        public bool HasComplexBans()
        {
            throw new NotImplementedException("HasComplexBans method is not implemented yet.");
        }
    }

    /// <summary>
    /// Represents a format effect.
    /// </summary>
    public interface IFormat : IBasicEffect, IEffect
    {
        public string Mod { get; set; }

        /// <summary>
        /// Name of the team generator algorithm, if this format uses
        /// random/fixed teams. null if players can bring teams.
        /// </summary>
        public string? Team { get; set; }

        public FormatEffectType FormatEffectType { get; set; }
        public bool Debug { get; set; }

        /// <summary>
        /// Whether or not a format will update ladder points if searched
        /// for using the "Battle!" button.
        /// (Challenge and tournament games will never update ladder points.)
        /// (Defaults to true.)
        /// </summary>
        public object Rated { get; set; }

        /// <summary>Game type.</summary>
        public GameType GameType { get; set; }

        /// <summary>Number of players, based on game type, for convenience</summary>
        public int PlayerCount { get; set; }

        /// <summary>List of rule names.</summary>
        public List<string> Ruleset { get; set; }

        /// <summary>
        /// Base list of rule names as specified in "./config/formats.ts".
        /// Used in a custom format to correctly display the altered ruleset.
        /// </summary>
        public List<string> BaseRuleset { get; set; }

        /// <summary>List of banned effects.</summary>
        public List<string> Banlist { get; set; }

        /// <summary>List of effects that aren't completely banned.</summary>
        public List<string> Restricted { get; set; }

        /// <summary>List of inherited banned effects to override.</summary>
        public List<string> Unbanlist { get; set; }

        /// <summary>List of ruleset and banlist changes in a custom format.</summary>
        public List<string>? CustomRules { get; set; }

        /// <summary>Table of rule names and banned effects.</summary>
        public RuleTable? RuleTable { get; set; }

        /// <summary>An optional function that runs at the start of a battle.</summary>
        public Action<Battle>? OnBegin { get; set; } // deault is undefined

        public bool NoLog { get; set; }

        // Rule-specific properties (only apply to rules, not formats)
        public object? HasValue { get; set; } // Can be bool, "integer", or "positive-integer"
        public Func<ValidationContext, string, string?>? OnValidateRule { get; set; }

        /// <summary>ID of rule that can't be combined with this rule</summary>
        public string? MutuallyExclusiveWith { get; set; }

        // Battle module properties
        public IModdedBattleScriptsData? Battle { get; set; }
        public IModdedBattlePokemon? Pokemon { get; set; }
        public IModdedBattleQueue? Queue { get; set; }
        public IModdedField? Field { get; set; }
        public IModdedBattleActions? Actions { get; set; }
        public IModdedBattleSide? Side { get; set; }

        // Display and tournament properties
        public bool? ChallengeShow { get; set; }
        public bool? SearchShow { get; set; }
        public bool? BestOfDefault { get; set; }
        public bool? TeraPreviewDefault { get; set; }
        public List<string>? Threads { get; set; }
        public bool? TournamentShow { get; set; }

        // Validation functions
        public Func<TeamValidator, Move, Species, PokemonSources, PokemonSet, string?>? CheckCanLearn { get; set; }
        public Func<Format, string, Id>? GetEvoFamily { get; set; }
        public Func<Format, Pokemon, HashSet<string>>? GetSharedPower { get; set; }
        public Func<Format, Pokemon, HashSet<string>>? GetSharedItems { get; set; }
        public Func<TeamValidator, PokemonSet, Format, object?, object?, List<string>?>? OnChangeSet { get; set; }

        // Battle event handlers
        public int? OnModifySpeciesPriority { get; set; }
        public Func<Battle, Species, Pokemon?, Pokemon?, IEffect?, Species?>? OnModifySpecies { get; set; }
        public Action<Battle>? OnBattleStart { get; set; }
        public Action<Battle>? OnTeamPreview { get; set; }
        public Func<TeamValidator, PokemonSet, Format, object, object, List<string>?>? OnValidateSet { get; set; }
        public Func<TeamValidator, List<PokemonSet>, Format, object, List<string>?>? OnValidateTeam { get; set; }
        public Func<TeamValidator, PokemonSet, object, List<string>?>? ValidateSet { get; set; }
        public Func<TeamValidator, List<PokemonSet>, ValidationOptions?, List<string>?>? ValidateTeam { get; set; }

        // Layout properties
        public string? Section { get; set; }
        public int? Column { get; set; }
    }

    public class Format : BasicEffect, IFormat
    {
        public string Mod { get; set; } = "gen9";
        public string? Team { get; set; } = null;
        public FormatEffectType FormatEffectType { get; set; } = FormatEffectType.Format;
        public bool Debug { get; set; } = false;
        public object Rated { get; set; } = false;
        public GameType GameType { get; set; } = GameType.Singles;
        public int PlayerCount { get; set; }
        public List<string> Ruleset { get; set; } = [];
        public List<string> BaseRuleset { get; set; } = [];
        public List<string> Banlist { get; set; } = [];
        public List<string> Restricted { get; set; } = [];
        public List<string> Unbanlist { get; set; } = [];
        public List<string>? CustomRules { get; set; } = null;
        public RuleTable? RuleTable { get; set; } = null;
        public Action<Battle>? OnBegin { get; set; }
        public bool NoLog { get; set; }
        public object? HasValue { get; set; }
        public Func<ValidationContext, string, string?>? OnValidateRule { get; set; }
        public string? MutuallyExclusiveWith { get; set; }
        public IModdedBattleScriptsData? Battle { get; set; }
        public IModdedBattlePokemon? Pokemon { get; set; }
        public IModdedBattleQueue? Queue { get; set; }
        public IModdedField? Field { get; set; }
        public IModdedBattleActions? Actions { get; set; }
        public IModdedBattleSide? Side { get; set; }
        public bool? ChallengeShow { get; set; }
        public bool? SearchShow { get; set; }
        public bool? BestOfDefault { get; set; }
        public bool? TeraPreviewDefault { get; set; }
        public List<string>? Threads { get; set; }
        public bool? TournamentShow { get; set; }
        public Func<TeamValidator, Move, Species, PokemonSources, PokemonSet, string?>? CheckCanLearn { get; set; }
        public Func<Format, string, Id>? GetEvoFamily { get; set; }
        public Func<Format, Pokemon, HashSet<string>>? GetSharedPower { get; set; }
        public Func<Format, Pokemon, HashSet<string>>? GetSharedItems { get; set; }
        public Func<TeamValidator, PokemonSet, Format, object?, object?, List<string>?>? OnChangeSet { get; set; }
        public int? OnModifySpeciesPriority { get; set; }
        public Func<Battle, Species, Pokemon?, Pokemon?, IEffect?, Species?>? OnModifySpecies { get; set; }
        public Action<Battle>? OnBattleStart { get; set; }
        public Action<Battle>? OnTeamPreview { get; set; }
        public Func<TeamValidator, PokemonSet, Format, object, object, List<string>?>? OnValidateSet { get; set; }
        public Func<TeamValidator, List<PokemonSet>, Format, object, List<string>?>? OnValidateTeam { get; set; }
        public Func<TeamValidator, PokemonSet, object, List<string>?>? ValidateSet { get; set; }
        public Func<TeamValidator, List<PokemonSet>, ValidationOptions?, List<string>?>? ValidateTeam { get; set; }
        public string? Section { get; set; }
        public int? Column { get; set; }

        public Format(IFormat data) : base(data)
        {
            Mod = data.Mod;
            Team = data.Team;
            FormatEffectType = data.FormatEffectType;
            Debug = data.Debug;
            Rated = data.Rated;
            GameType = data.GameType;
            PlayerCount = data.PlayerCount;
            Ruleset = data.Ruleset;
            BaseRuleset = data.BaseRuleset;
            Banlist = data.Banlist;
            Restricted = data.Restricted;
            Unbanlist = data.Unbanlist;
            CustomRules = data.CustomRules;
            RuleTable = data.RuleTable;
            OnBegin = data.OnBegin;
            NoLog = data.NoLog;
            HasValue = data.HasValue;
            OnValidateRule = data.OnValidateRule;
            MutuallyExclusiveWith = data.MutuallyExclusiveWith;
            Battle = data.Battle;
            Pokemon = data.Pokemon;
            Queue = data.Queue;
            Field = data.Field;
            Actions = data.Actions;
            Side = data.Side;
            ChallengeShow = data.ChallengeShow;
            SearchShow = data.SearchShow;
            BestOfDefault = data.BestOfDefault;
            TeraPreviewDefault = data.TeraPreviewDefault;
            Threads = data.Threads;
            TournamentShow = data.TournamentShow;
            CheckCanLearn = data.CheckCanLearn;
            GetEvoFamily = data.GetEvoFamily;
            GetSharedPower = data.GetSharedPower;
            GetSharedItems = data.GetSharedItems;
            OnChangeSet = data.OnChangeSet;
            OnModifySpeciesPriority = data.OnModifySpeciesPriority;
            OnModifySpecies = data.OnModifySpecies;
            OnBattleStart = data.OnBattleStart;
            OnTeamPreview = data.OnTeamPreview;
            OnValidateSet = data.OnValidateSet;
            OnValidateTeam = data.OnValidateTeam;
            ValidateSet = data.ValidateSet;
            ValidateTeam = data.ValidateTeam;
            Section = data.Section;
            Column = data.Column;
        }

        public void Init()
        {
            InitBasicEffect();

            if (!(Rated.GetType() == typeof(bool) || Rated.GetType() == typeof(string)))
            {
                throw new ArgumentException("Rated must be a bool or string.");
            }

            if (GameType == GameType.Multi || GameType == GameType.FreeForAll)
            {
                PlayerCount = 4;
            }
            else
            {
                PlayerCount = 2;
            }
        }
    }

    // Helper class for Format
    public class ValidationContext
        {
            public Format Format { get; set; } = null!;
            public RuleTable RuleTable { get; set; } = null!;
            public ModdedDex Dex { get; set; } = null!;
        }

        // Helper class for Format
        public class ValidationOptions
        {
            public bool? RemoveNicknames { get; set; }
            public Dictionary<string, Dictionary<string, bool>>? SkipSets { get; set; }
        }

    public static class FormatUtils
    {
        // function mergeFormatLists(main: FormatList, custom: FormatList | undefined): FormatList
        public static FormatList MergeFormatLists(FormatList main, FormatList? custom)
        {
            throw new NotImplementedException("MergeFormatLists method is not implemented yet.");
        }
    }

    public class DexFormats(ModdedDex dex)
    {
        ModdedDex Dex { get; } = dex;
        private readonly Dictionary<Id, Format> _rulesetCache = [];
        private readonly Format[]? _formatListCache = null;

        public DexFormats Load()
        {
            throw new NotImplementedException("load method is not implemented yet.");
        }

        public string Validate(string name)
        {
            throw new NotImplementedException("Validate method is not implemented yet.");
        }

        public Format Get(string? name = null, bool isTrusted = false)
        {
            throw new NotImplementedException();
        }

        public Format[] All()
        {
            throw new NotImplementedException("All method is not implemented yet.");
        }

        public bool IsPokemonRule(string ruleSpec)
        {
            throw new NotImplementedException("IsPokemonRule method is not implemented yet.");
        }

        public RuleTable GetRuleTable(Format format, int depth = 1, Dictionary<string, int>? repeals = null)
        {
            throw new NotImplementedException("GetRuleTable method is not implemented yet.");
        }

        public object ValidateRule(string rule, Format? format = null)
        {
            throw new NotImplementedException("ValidateRule method is not implemented yet.");
        }

        public bool ValidPokemonTag(Id tagId)
        {
            throw new NotImplementedException("ValidPokemonTag method is not implemented yet.");
        }

        public string ValidateBanRule(string rule)
        {
            throw new NotImplementedException("ValidateBanRule method is not implemented yet.");
        }
    }
}