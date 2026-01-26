using System.Collections.ObjectModel;
using ApogeeVGC.Sim.FormatClasses;

namespace ApogeeVGC.Data;

/// <summary>
/// Defines reusable rulesets that can be referenced by formats.
/// Based on Pokemon Showdown's data/rulesets.ts.
/// </summary>
public record Rulesets
{
    public IReadOnlyDictionary<RuleId, Format> RulesetData { get; }

    public Rulesets()
    {
        RulesetData = new ReadOnlyDictionary<RuleId, Format>(_rulesetData);
    }

    private readonly Dictionary<RuleId, Format> _rulesetData = new()
    {
        [RuleId.Standard] = new Format
        {
            FormatEffectType = FormatEffectType.ValidatorRule,
            Name = "Standard",
            Desc = "The standard ruleset for all official Smogon singles tiers (Ubers, OU, etc.)",
            Ruleset = [],
        },

        [RuleId.StandardDoubles] = new Format
        {
            FormatEffectType = FormatEffectType.ValidatorRule,
            Name = "Standard Doubles",
            Desc = "The standard ruleset for all official Smogon doubles tiers",
            Ruleset = [RuleId.Obtainable, RuleId.TeamPreview, RuleId.HpPercentageMod, RuleId.CancelMod,
                       RuleId.EndlessBattleClause, RuleId.SpeciesClause, RuleId.NicknameClause],
        },

        [RuleId.FlatRules] = new Format
        {
            FormatEffectType = FormatEffectType.ValidatorRule,
            Name = "Flat Rules",
            Desc = "The in-game Flat Rules: Adjust Level Down 50, Species Clause, Item Clause = 1, -Mythical, -Restricted Legendary, Bring 6 Pick 4.",
            Ruleset = [RuleId.Obtainable, RuleId.TeamPreview, RuleId.SpeciesClause, RuleId.NicknameClause, RuleId.ItemClause, RuleId.AdjustLevelDown, RuleId.CancelMod],
            Banlist = [RuleId.Mythical, RuleId.RestrictedLegendary],
        },

        [RuleId.Obtainable] = new Format
        {
            FormatEffectType = FormatEffectType.ValidatorRule,
            Name = "Obtainable",
            Desc = "Makes sure the team is possible to obtain in-game.",
            Ruleset = [RuleId.ObtainableMoves, RuleId.ObtainableAbilities, RuleId.ObtainableFormes, RuleId.ObtainableMisc],
        },

        [RuleId.TeamPreview] = new Format
        {
            FormatEffectType = FormatEffectType.Rule,
            Name = "Team Preview",
            Desc = "Allows each player to see the Pokémon on their opponent's team before battle.",
        },

        [RuleId.SpeciesClause] = new Format
        {
            FormatEffectType = FormatEffectType.ValidatorRule,
            Name = "Species Clause",
            Desc = "Prevents teams from having more than one Pokémon from the same species.",
        },

        [RuleId.NicknameClause] = new Format
        {
            FormatEffectType = FormatEffectType.ValidatorRule,
            Name = "Nickname Clause",
            Desc = "Prevents teams from having two Pokémon with the same nickname.",
        },

        [RuleId.ItemClause] = new Format
        {
            FormatEffectType = FormatEffectType.ValidatorRule,
            Name = "Item Clause",
            Desc = "Prevents teams from having more than one Pokémon holding the same item.",
        },

        [RuleId.VGCTimer] = new Format
        {
            FormatEffectType = FormatEffectType.Rule,
            Name = "VGC Timer",
            Desc = "VGC's timer: 90 second Team Preview, 7 minutes Your Time, 1 minute per turn.",
        },

        [RuleId.LimitOneRestricted] = new Format
        {
            FormatEffectType = FormatEffectType.ValidatorRule,
            Name = "Limit One Restricted",
            Desc = "Limit one restricted Pokémon (flagged with * in the rules list).",
        },

        [RuleId.LimitTwoRestricted] = new Format
        {
            FormatEffectType = FormatEffectType.ValidatorRule,
            Name = "Limit Two Restricted",
            Desc = "Limit two restricted Pokémon (flagged with * in the rules list).",
        },

        [RuleId.EndlessBattleClause] = new Format
        {
            FormatEffectType = FormatEffectType.Rule,
            Name = "Endless Battle Clause",
            Desc = "Prevents battles from going on indefinitely.",
        },

        [RuleId.CancelMod] = new Format
        {
            FormatEffectType = FormatEffectType.Rule,
            Name = "Cancel Mod",
            Desc = "Allows players to cancel their move selection before the turn starts.",
        },

        [RuleId.HpPercentageMod] = new Format
        {
            FormatEffectType = FormatEffectType.Rule,
            Name = "HP Percentage Mod",
            Desc = "Shows HP as a percentage instead of exact numbers.",
        },

        [RuleId.AdjustLevelDown] = new Format
        {
            FormatEffectType = FormatEffectType.ValidatorRule,
            Name = "Adjust Level Down",
            Desc = "Adjusts Pokémon above a certain level down to that level (typically 50 for VGC).",
        },
    };
}