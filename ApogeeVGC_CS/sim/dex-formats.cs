using System;
using System.Collections.Generic;
using System.Data.Common;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC_CS.sim
{
    public class FormatData(IAnyObject data) : Format(data), IEventMethods
    {

    }

    // Represents the element of a "FormatList" type
    public interface IFormatListEntry { }

    public class FormatDataEntry(IAnyObject data) : FormatData(data), IFormatListEntry
    {
    }

    public class SectionEntry : IFormatListEntry
    {
        public string Section { get; set; } = string.Empty;
        public int? Column { get; set; }
    }

    public class FormatDataTable : Dictionary<IdEntry, FormatData>
    {

    }

    // Should really use ModdedFormatData instead of FormatData
    public class ModdedFormatDataTable : Dictionary<IdEntry, FormatData>
    {

    }

    public enum FormatEffectType
    {
        Format,
        Ruleset,
        Rule,
        ValidatorRule
    }

    public class ComplexBan
        {
        public string Rule { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public int Limit { get; set; }
        public List<string> Bans { get; set; } = new();
    }

    public class GameTimerSettings
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

    public class RuleTable : Dictionary<string, string>
    {
        public List<ComplexBan> ComplexBans { get; set; } = [];
        public List<ComplexBan> ComplexTeamBans { get; set; } = [];
        public (Func<object, bool> CheckCanLearn, string)? CheckCanLearn { get; set; }
        public (GameTimerSettings Timer, string)? Timer { get; set; }
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

        public RuleTable()
        {
            ComplexBans = [];
            ComplexTeamBans = [];
            TagRules = [];
            ValueRules = [];
        }

        // TODO - add methods
    }

    public class Format : BasicEffect, IFormat
    {
        public Format(IAnyObject data) : base(data)
        {
        }
        // TODO - add properties and methods
    }

    public class DexFormats
    {
        // TODO - add properties and methods for DexFormats
    }
}