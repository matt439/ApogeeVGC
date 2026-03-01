using System.Collections.Frozen;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;

namespace ApogeeVGC.Data;

/// <summary>
/// Defines all available battle formats.
/// VGC formats are based on Pokemon Showdown's format system with Gen 9 Regulations A-I.
/// </summary>
public record Formats
{
    public FrozenDictionary<FormatId, Format> FormatData { get; }

    public Formats()
    {
        FormatData = _formatData.ToFrozenDictionary();
    }

    private readonly Dictionary<FormatId, Format> _formatData = new()
    {
        // Smogon formats
        [FormatId.Gen9Ou] = new Format
        {
            Name = "[Gen 9] OU",
            Ruleset = [RuleId.Standard],
            Banlist = [],
        },

        // Custom formats for testing
        [FormatId.CustomSingles] = new Format
        {
            Name = "Custom Singles",
            Ruleset = [],
            Banlist = [],
            RuleTable = new RuleTable
            {
                PickedTeamSize = 6,
            },
        },
        [FormatId.CustomSinglesBlind] = new Format
        {
            Name = "Custom Singles (Blind)",
            Ruleset = [],
            Banlist = [],
            RuleTable = new RuleTable
            {
                PickedTeamSize = 0,
            },
        },
        [FormatId.CustomDoubles] = new Format
        {
            Name = "Custom Doubles",
            GameType = GameType.Doubles,
            Ruleset = [],
            Banlist = [],
            RuleTable = new RuleTable
            {
                PickedTeamSize = 4,
                AllowMegaEvolution = true,
            },
        },

        // ===========================================
        // VGC Gen 9 Regulations A-I
        // ===========================================

        // Regulation A (Dec 2022 - Jan 2023): Paldea dex only, no restricted
        [FormatId.Gen9VgcRegulationA] = new Format
        {
            Name = "[Gen 9] VGC 2023 Regulation A",
            GameType = GameType.Doubles,
            Ruleset = [RuleId.FlatRules, RuleId.VgcTimer],
            Banlist = [RuleId.Mythical, RuleId.RestrictedLegendary],
            RuleTable = new RuleTable
            {
                MinTeamSize = 4,
                MaxTeamSize = 6,
                PickedTeamSize = 4,
                AdjustLevelDown = 50,
                MaxMoveCount = 4,
                EvLimit = 510,
            },
        },

        // Regulation B (Feb - Mar 2023): Paldea dex only, no restricted
        [FormatId.Gen9VgcRegulationB] = new Format
        {
            Name = "[Gen 9] VGC 2023 Regulation B",
            GameType = GameType.Doubles,
            Ruleset = [RuleId.FlatRules, RuleId.VgcTimer],
            Banlist = [RuleId.Mythical, RuleId.RestrictedLegendary],
            RuleTable = new RuleTable
            {
                MinTeamSize = 4,
                MaxTeamSize = 6,
                PickedTeamSize = 4,
                AdjustLevelDown = 50,
                MaxMoveCount = 4,
                EvLimit = 510,
            },
        },

        // Regulation C (Apr - Aug 2023): Paldea + Teal Mask, no restricted
        [FormatId.Gen9VgcRegulationC] = new Format
        {
            Name = "[Gen 9] VGC 2023 Regulation C",
            GameType = GameType.Doubles,
            Ruleset = [RuleId.FlatRules, RuleId.VgcTimer],
            Banlist = [RuleId.Mythical, RuleId.RestrictedLegendary],
            RuleTable = new RuleTable
            {
                MinTeamSize = 4,
                MaxTeamSize = 6,
                PickedTeamSize = 4,
                AdjustLevelDown = 50,
                MaxMoveCount = 4,
                EvLimit = 510,
            },
        },

        // Regulation D (Sep - Dec 2023): All obtainable, 1 restricted
        [FormatId.Gen9VgcRegulationD] = new Format
        {
            Name = "[Gen 9] VGC 2023 Regulation D",
            GameType = GameType.Doubles,
            Ruleset = [RuleId.FlatRules, RuleId.VgcTimer, RuleId.LimitOneRestricted],
            Banlist = [RuleId.Mythical],
            RuleTable = new RuleTable
            {
                MinTeamSize = 4,
                MaxTeamSize = 6,
                PickedTeamSize = 4,
                AdjustLevelDown = 50,
                MaxMoveCount = 4,
                EvLimit = 510,
            },
        },

        // Regulation E (Jan - Mar 2024): All obtainable + Indigo Disk, 1 restricted
        [FormatId.Gen9VgcRegulationE] = new Format
        {
            Name = "[Gen 9] VGC 2024 Regulation E",
            GameType = GameType.Doubles,
            Ruleset = [RuleId.FlatRules, RuleId.VgcTimer, RuleId.LimitOneRestricted],
            Banlist = [RuleId.Mythical],
            RuleTable = new RuleTable
            {
                MinTeamSize = 4,
                MaxTeamSize = 6,
                PickedTeamSize = 4,
                AdjustLevelDown = 50,
                MaxMoveCount = 4,
                EvLimit = 510,
            },
        },

        // Regulation F (Apr - Aug 2024): All obtainable, 2 restricted
        [FormatId.Gen9VgcRegulationF] = new Format
        {
            Name = "[Gen 9] VGC 2024 Regulation F",
            GameType = GameType.Doubles,
            Ruleset = [RuleId.FlatRules, RuleId.VgcTimer, RuleId.LimitTwoRestricted],
            Banlist = [RuleId.Mythical],
            RuleTable = new RuleTable
            {
                MinTeamSize = 4,
                MaxTeamSize = 6,
                PickedTeamSize = 4,
                AdjustLevelDown = 50,
                MaxMoveCount = 4,
                EvLimit = 510,
            },
        },

        // Regulation G (Sep - Dec 2024, 2025): All obtainable, 2 restricted
        [FormatId.Gen9VgcRegulationG] = new Format
        {
            Name = "[Gen 9] VGC 2025 Regulation G",
            GameType = GameType.Doubles,
            Ruleset = [RuleId.FlatRules, RuleId.VgcTimer, RuleId.LimitTwoRestricted],
            Banlist = [RuleId.Mythical],
            RuleTable = new RuleTable
            {
                MinTeamSize = 4,
                MaxTeamSize = 6,
                PickedTeamSize = 4,
                AdjustLevelDown = 50,
                MaxMoveCount = 4,
                EvLimit = 510,
            },
        },

        // Regulation H (Jan - Apr 2025): All obtainable, 1 restricted
        [FormatId.Gen9VgcRegulationH] = new Format
        {
            Name = "[Gen 9] VGC 2025 Regulation H",
            GameType = GameType.Doubles,
            Ruleset = [RuleId.FlatRules, RuleId.VgcTimer, RuleId.LimitOneRestricted],
            Banlist = [RuleId.Mythical],
            RuleTable = new RuleTable
            {
                MinTeamSize = 4,
                MaxTeamSize = 6,
                PickedTeamSize = 4,
                AdjustLevelDown = 50,
                MaxMoveCount = 4,
                EvLimit = 510,
            },
        },

        // Regulation I (May 2025+): All obtainable, no restricted
        [FormatId.Gen9VgcRegulationI] = new Format
        {
            Name = "[Gen 9] VGC 2025 Regulation I",
            GameType = GameType.Doubles,
            Ruleset = [RuleId.FlatRules, RuleId.VgcTimer],
            Banlist = [RuleId.Mythical, RuleId.RestrictedLegendary],
            RuleTable = new RuleTable
            {
                MinTeamSize = 4,
                MaxTeamSize = 6,
                PickedTeamSize = 4,
                AdjustLevelDown = 50,
                MaxMoveCount = 4,
                EvLimit = 510,
            },
        },
        // ===========================================
        // Mega Evolution Formats
        // ===========================================

        [FormatId.Gen9VgcMega] = new Format
        {
            Name = "[Gen 9] VGC Mega Evolution",
            GameType = GameType.Doubles,
            Ruleset = [RuleId.FlatRules, RuleId.VgcTimer],
            Banlist = [RuleId.Mythical, RuleId.RestrictedLegendary],
            RuleTable = new RuleTable
            {
                MinTeamSize = 4,
                MaxTeamSize = 6,
                PickedTeamSize = 4,
                AdjustLevelDown = 50,
                MaxMoveCount = 4,
                EvLimit = 510,
                AllowMegaEvolution = true,
            },
        },
    };
}