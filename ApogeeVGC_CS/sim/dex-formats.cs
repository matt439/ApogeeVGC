using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC_CS.sim
{
    public interface IFormatData : IEventMethods, IFormatListEntry
    {
        // TS version also extends Partial<Format>
        // Define methods from Format that should be implemented
    }

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
    public interface IFormatDataTable : IDictionary<IdEntry, IFormatData>
    {
    }

    // export interface ModdedFormatDataTable { [id: IDEntry]: ModdedFormatData }
    public interface IModdedFormatDataTable : IDictionary<IdEntry, IModdedFormatData>
    {
    }

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

    public class Format : BasicEffect, IFormat
    {
        // Basic format properties
        public string Mod { get; } = "gen9"; // Default value

        /// <summary>
        /// Name of the team generator algorithm, if this format uses
        /// random/fixed teams. null if players can bring teams.
        /// </summary>
        public string? Team { get; } = null;

        public FormatEffectType FormatEffectType { get; } = FormatEffectType.Format; // Default value
        public bool Debug { get; } = false; // Default value

        /// <summary>
        /// Whether or not a format will update ladder points if searched
        /// for using the "Battle!" button.
        /// (Challenge and tournament games will never update ladder points.)
        /// (Defaults to true.)
        /// </summary>
        public object Rated { get; } = true; // Can be bool or string, defaults to true

        /// <summary>Game type.</summary>
        public GameType GameType { get; } = GameType.Singles; // Default value

        /// <summary>Number of players, based on game type, for convenience</summary>
        public int PlayerCount { get; }

        /// <summary>List of rule names.</summary>
        public List<string> Ruleset { get; } = []; // Default to empty list

        /// <summary>
        /// Base list of rule names as specified in "./config/formats.ts".
        /// Used in a custom format to correctly display the altered ruleset.
        /// </summary>
        public List<string> BaseRuleset { get; } = [];

        /// <summary>List of banned effects.</summary>
        public List<string> Banlist { get; } = [];

        /// <summary>List of effects that aren't completely banned.</summary>
        public List<string> Restricted { get; } = [];

        /// <summary>List of inherited banned effects to override.</summary>
        public List<string> Unbanlist { get; } = [];

        /// <summary>List of ruleset and banlist changes in a custom format.</summary>
        public List<string>? CustomRules { get; } = null;

        /// <summary>Table of rule names and banned effects.</summary>
        public RuleTable? RuleTable { get; set; } = null;

        /// <summary>An optional function that runs at the start of a battle.</summary>
        public Action<Battle>? OnBegin { get; } // deault is undefined

        public bool NoLog { get; }

        // Rule-specific properties (only apply to rules, not formats)
        public object? HasValue { get; } // Can be bool, "integer", or "positive-integer"
        public Func<ValidationContext, string, string?>? OnValidateRule { get; }

        /// <summary>ID of rule that can't be combined with this rule</summary>
        public string? MutuallyExclusiveWith { get; }

        // Battle module properties
        public IModdedBattleScriptsData? Battle { get; }
        public IModdedBattlePokemon? Pokemon { get; }
        public IModdedBattleQueue? Queue { get; }
        public IModdedField? Field { get; }
        public IModdedBattleActions? Actions { get; }
        public IModdedBattleSide? Side { get; }

        // Display and tournament properties
        public bool? ChallengeShow { get; }
        public bool? SearchShow { get; }
        public bool? BestOfDefault { get; }
        public bool? TeraPreviewDefault { get; }
        public List<string>? Threads { get; }
        public bool? TournamentShow { get; }

        // Validation functions
        public Func<TeamValidator, Move, Species, PokemonSources, PokemonSet, string?>? CheckCanLearn { get; }
        public Func<Format, string, Id>? GetEvoFamily { get; }
        public Func<Format, Pokemon, HashSet<string>>? GetSharedPower { get; }
        public Func<Format, Pokemon, HashSet<string>>? GetSharedItems { get; }
        public Func<TeamValidator, PokemonSet, Format, IAnyObject?, IAnyObject?, List<string>?>? OnChangeSet { get; }

        // Battle event handlers
        public int? OnModifySpeciesPriority { get; }
        public Func<Battle, Species, Pokemon?, Pokemon?, IEffect?, Species?>? OnModifySpecies { get; }
        public Action<Battle>? OnBattleStart { get; }
        public Action<Battle>? OnTeamPreview { get; }
        public Func<TeamValidator, PokemonSet, Format, IAnyObject, IAnyObject, List<string>?>? OnValidateSet { get; }
        public Func<TeamValidator, List<PokemonSet>, Format, IAnyObject, List<string>?>? OnValidateTeam { get; }
        public Func<TeamValidator, PokemonSet, IAnyObject, List<string>?>? ValidateSet { get; }
        public Func<TeamValidator, List<PokemonSet>, ValidationOptions?, List<string>?>? ValidateTeam { get; }

        // Layout properties
        public string? Section { get; }
        public int? Column { get; }

        public Format(IAnyObject data) : base(data)
        {
            if (data.TryGetString("mod", out var mod))
            {
                Mod = mod;
            }

            if (data.TryGetEnum<FormatEffectType>("effectType", out var effectType))
            {
                FormatEffectType = effectType;
            }

            if (data.TryGetBool("debug", out var debug))
            {
                Debug = debug;
            }

            // Rated can be a boolean or a string
            if (data.TryGetString("rated", out var ratedString))
            {
                Rated = ratedString;
            }
            else if (data.TryGetBool("rated", out var ratedBool))
            {
                Rated = ratedBool;
            }

            if (data.TryGetEnum<GameType>("gameType", out var gameType))
            {
                GameType = gameType;
            }

            if (data.TryGetList<string>("ruleset", out var ruleset))
            {
                Ruleset = ruleset;
            }

            if (data.TryGetList<string>("baseRuleset", out var baseRuleset))
            {
                BaseRuleset = baseRuleset;
            }

            if (data.TryGetList<string>("banlist", out var banlist))
            {
                Banlist = banlist;
            }

            if (data.TryGetList<string>("restricted", out var restricted))
            {
                Restricted = restricted;
            }

            if (data.TryGetList<string>("unbanlist", out var unbanlist))
            {
                Unbanlist = unbanlist;
            }

            if (data.TryGetList<string>("customRules", out var customRules))
            {
                CustomRules = customRules;
            }

            if (data.TryGetClass<RuleTable> ("ruleTable", out var ruleTable))
            {
                RuleTable = ruleTable;
            }

            if (data.TryGetAction<Battle>("onBegin", out var onBegin))
            {
                OnBegin = onBegin;
            }

            if (data.TryGetBool("noLog", out var noLog))
            {
                NoLog = noLog;
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