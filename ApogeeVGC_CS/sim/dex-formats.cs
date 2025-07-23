using System;
using System.Collections.Generic;
using System.Data.Common;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC_CS.sim
{
    // Represents a complex ban (rule, source, limit, bans)
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

    // RuleTable keeps track of the rules for a format
    public class RuleTable : Dictionary<string, string>
    {
        public List<ComplexBan> ComplexBans { get; set; } = new();
        public List<ComplexBan> ComplexTeamBans { get; set; } = new();
        public (Func<object, bool> CheckCanLearn, string)? CheckCanLearn { get; set; }
        public (GameTimerSettings Timer, string)? Timer { get; set; }
        public List<string> TagRules { get; set; } = new();
        public Dictionary<string, string> ValueRules { get; set; } = new();

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
            ComplexBans = new List<ComplexBan>();
            ComplexTeamBans = new List<ComplexBan>();
            TagRules = new List<string>();
            ValueRules = new Dictionary<string, string>();
        }

        // TODO - add methods
    }

    public class Format : BasicEffect
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