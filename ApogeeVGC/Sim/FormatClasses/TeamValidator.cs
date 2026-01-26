using ApogeeVGC.Data;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Sim.FormatClasses;

/// <summary>
/// Result of team or set validation.
/// </summary>
/// <param name="IsValid">True if validation passed with no problems.</param>
/// <param name="Problems">List of validation error messages. Empty if valid.</param>
public record ValidationResult(bool IsValid, IReadOnlyList<string> Problems)
{
    public static ValidationResult Success { get; } = new(true, []);

    public static ValidationResult Failure(params string[] problems) => new(false, problems);

    public static ValidationResult Failure(List<string> problems) => new(false, problems);
}

/// <summary>
/// Validates Pokemon teams against format rules.
/// Uses RuleTable properties for dynamic rule enforcement based on format.
/// </summary>
public class TeamValidator
{
    private const int Gen9 = 9;

    private readonly Library _library;
    private readonly Format _format;
    private readonly RuleTable _ruleTable;

    /// <summary>
    /// Creates a TeamValidator for the specified format.
    /// </summary>
    /// <param name="library">The game data library.</param>
    /// <param name="format">The format to validate against.</param>
    public TeamValidator(Library library, Format format)
    {
        ArgumentNullException.ThrowIfNull(library);
        ArgumentNullException.ThrowIfNull(format);

        _library = library;
        _format = format;

        // Use format's RuleTable or create default
        _ruleTable = format.RuleTable ?? new RuleTable();
    }

    /// <summary>
    /// Validates a complete team of Pokemon against format rules.
    /// </summary>
    /// <param name="team">The team to validate.</param>
    /// <returns>Validation result with any problems found.</returns>
    public ValidationResult ValidateTeam(IReadOnlyList<PokemonSet> team)
    {
        ArgumentNullException.ThrowIfNull(team);

        var problems = new List<string>();

        // Team size validation using RuleTable
        if (team.Count < _ruleTable.MinTeamSize)
        {
            problems.Add($"Team must have at least {_ruleTable.MinTeamSize} Pokemon (your team has {team.Count}).");
            return ValidationResult.Failure(problems);
        }

        if (team.Count > _ruleTable.MaxTeamSize)
        {
            problems.Add($"Team cannot have more than {_ruleTable.MaxTeamSize} Pokemon (your team has {team.Count}).");
            return ValidationResult.Failure(problems);
        }

        // Item Clause: Each item must be unique (if rule is active)
        if (_ruleTable.Has(RuleId.ItemClause))
        {
            ValidateItemClause(team, problems);
        }

        // Species Clause: Each species must be unique (if rule is active)
        if (_ruleTable.Has(RuleId.SpeciesClause))
        {
            ValidateSpeciesClause(team, problems);
        }

        // Validate each individual Pokemon set
        foreach (var set in team)
        {
            var setResult = ValidateSet(set);
            if (!setResult.IsValid)
            {
                problems.AddRange(setResult.Problems);
            }
        }

        return problems.Count == 0 ? ValidationResult.Success : ValidationResult.Failure(problems);
    }

    /// <summary>
    /// Validates Item Clause: each item must be unique (except no item).
    /// </summary>
    private void ValidateItemClause(IReadOnlyList<PokemonSet> team, List<string> problems)
    {
        var itemCounts = new Dictionary<ItemId, int>();
        foreach (var set in team)
        {
            if (set.Item != ItemId.None)
            {
                itemCounts.TryGetValue(set.Item, out var count);
                itemCounts[set.Item] = count + 1;
            }
        }

        foreach (var (item, count) in itemCounts)
        {
            if (count > 1)
            {
                var itemName = _library.Items.TryGetValue(item, out var itemData) ? itemData.Name : item.ToString();
                problems.Add($"Item Clause: You are limited to one of each item. {itemName} appears {count} times.");
            }
        }
    }

    /// <summary>
    /// Validates Species Clause: each species must be unique.
    /// </summary>
    private void ValidateSpeciesClause(IReadOnlyList<PokemonSet> team, List<string> problems)
    {
        var speciesCounts = new Dictionary<SpecieId, int>();
        foreach (var set in team)
        {
            var species = _library.Species.GetValueOrDefault(set.Species);
            var baseSpeciesId = species?.BaseSpecies ?? set.Species;

            speciesCounts.TryGetValue(baseSpeciesId, out var count);
            speciesCounts[baseSpeciesId] = count + 1;
        }

        foreach (var (speciesId, count) in speciesCounts)
        {
            if (count > 1)
            {
                var speciesName = _library.Species.TryGetValue(speciesId, out var s) ? s.Name : speciesId.ToString();
                problems.Add(
                    $"Species Clause: You are limited to one of each Pokemon species. {speciesName} appears {count} times.");
            }
        }
    }

    /// <summary>
    /// Validates an individual Pokemon set.
    /// </summary>
    /// <param name="set">The Pokemon set to validate.</param>
    /// <returns>Validation result with any problems found.</returns>
    public ValidationResult ValidateSet(PokemonSet set)
    {
        ArgumentNullException.ThrowIfNull(set);

        var problems = new List<string>();
        var name = set.Name;

        // Species existence check
        if (!_library.Species.TryGetValue(set.Species, out var species))
        {
            problems.Add($"The Pokemon \"{set.Species}\" does not exist.");
            return ValidationResult.Failure(problems);
        }

        // Level validation using RuleTable
        if (set.Level < _ruleTable.MinLevel)
        {
            problems.Add($"{name} is level {set.Level}, but minimum level is {_ruleTable.MinLevel}.");
        }

        if (set.Level > _ruleTable.MaxLevel)
        {
            problems.Add($"{name} is level {set.Level}, but maximum level is {_ruleTable.MaxLevel}.");
        }

        // Ability validation
        var abilityProblem = ValidateAbility(set.Ability, species, name);
        if (abilityProblem != null)
        {
            problems.Add(abilityProblem);
        }

        // Move validation
        foreach (var moveId in set.Moves)
        {
            var moveProblem = ValidateMove(moveId, set.Species, species, name, set.Level);
            if (moveProblem != null)
            {
                problems.Add(moveProblem);
            }
        }

        // Move count validation
        if (set.Moves.Count > _ruleTable.MaxMoveCount)
        {
            problems.Add($"{name} has {set.Moves.Count} moves, but maximum is {_ruleTable.MaxMoveCount}.");
        }

        // Item existence check (if item is specified)
        if (set.Item != ItemId.None && !_library.Items.ContainsKey(set.Item))
        {
            problems.Add($"{name}'s item \"{set.Item}\" does not exist.");
        }

        return problems.Count == 0 ? ValidationResult.Success : ValidationResult.Failure(problems);
    }

    /// <summary>
    /// Validates that a Pokemon can have the specified ability.
    /// </summary>
    private string? ValidateAbility(AbilityId abilityId, Species species, string pokemonName)
    {
        if (abilityId == AbilityId.None)
        {
            return $"{pokemonName} needs to have an ability.";
        }

        // Check if ability exists
        if (!_library.Abilities.ContainsKey(abilityId))
        {
            return $"{pokemonName}'s ability \"{abilityId}\" does not exist.";
        }

        // Check if species can have this ability
        var abilities = species.Abilities;
        if (abilities.Slot0 != abilityId &&
            abilities.Slot1 != abilityId &&
            abilities.Hidden != abilityId &&
            abilities.Special != abilityId)
        {
            return $"{pokemonName} can't have {abilityId}.";
        }

        return null;
    }

    /// <summary>
    /// Validates that a Pokemon can learn the specified move.
    /// </summary>
    private string? ValidateMove(MoveId moveId, SpecieId speciesId, Species species, string pokemonName, int level)
    {
        // Check if move exists
        if (!_library.Moves.TryGetValue(moveId, out var move))
        {
            return $"\"{moveId}\" is an invalid move.";
        }

        // Check learnset
        if (!_library.Learnsets.TryGetValue(speciesId, out var learnset))
        {
            // Try base species if this is a forme
            if (species.BaseSpecies != speciesId &&
                _library.Learnsets.TryGetValue(species.BaseSpecies, out learnset))
            {
                // Use base species learnset
            }
            else
            {
                return $"{pokemonName} can't learn any moves (no learnset data found).";
            }
        }

        if (learnset.LearnsetData == null || !learnset.LearnsetData.TryGetValue(moveId, out var sources))
        {
            return $"{pokemonName} can't learn {move.Name}.";
        }

        // Check if any source is valid for Gen 9
        var canLearn = false;
        string? levelProblem = null;

        foreach (var source in sources)
        {
            // Only consider Gen 9 sources for VGC
            if (source.Generation != Gen9)
            {
                continue;
            }

            // For level-up moves, check if Pokemon is high enough level
            if (source is { SourceType: MoveSourceType.LevelUp, LevelOrIndex: not null })
            {
                if (level >= source.LevelOrIndex.Value)
                {
                    canLearn = true;
                    break;
                }
                else
                {
                    levelProblem = $"{pokemonName}'s move {move.Name} is learned at level {source.LevelOrIndex.Value}.";
                }
            }
            else
            {
                // TM, Tutor, Egg moves, etc. are always learnable
                canLearn = true;
                break;
            }
        }

        if (!canLearn)
        {
            // Return level problem if that was the only issue
            if (levelProblem != null)
            {
                return levelProblem;
            }

            return $"{pokemonName} can't learn {move.Name}.";
        }

        return null;
    }
}

public class PokemonSources;