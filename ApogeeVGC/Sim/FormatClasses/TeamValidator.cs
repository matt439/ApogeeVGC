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
/// Validates Pokemon teams for Gen 9 VGC format.
/// Checks move legality, ability validity, and format-specific rules like Item Clause.
/// </summary>
public class TeamValidator
{
    private const int MinTeamSize = 1;
    private const int MaxTeamSize = 6;
    private const int MaxMoveCount = 4;
    private const int Gen9 = 9;

    private readonly Library _library;

    public TeamValidator(Library library)
    {
        ArgumentNullException.ThrowIfNull(library);
        _library = library;
    }

    /// <summary>
    /// Validates a complete team of Pokemon.
    /// </summary>
    /// <param name="team">The team to validate.</param>
    /// <returns>Validation result with any problems found.</returns>
    public ValidationResult ValidateTeam(IReadOnlyList<PokemonSet> team)
    {
        ArgumentNullException.ThrowIfNull(team);

        var problems = new List<string>();

        // Team size validation
        if (team.Count < MinTeamSize)
        {
            problems.Add($"Team must have at least {MinTeamSize} Pokemon (your team has {team.Count}).");
            return ValidationResult.Failure(problems);
        }

        if (team.Count > MaxTeamSize)
        {
            problems.Add($"Team cannot have more than {MaxTeamSize} Pokemon (your team has {team.Count}).");
            return ValidationResult.Failure(problems);
        }

        // Item Clause: Each item must be unique (except no item)
        var itemCounts = new Dictionary<ItemId, int>();
        foreach (var set in team)
        {
            if (set.Item != ItemId.None)
            {
                itemCounts.TryGetValue(set.Item, out int count);
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

        // Species Clause: Each species must be unique
        var speciesCounts = new Dictionary<SpecieId, int>();
        foreach (var set in team)
        {
            var species = _library.Species.TryGetValue(set.Species, out var s) ? s : null;
            var baseSpeciesId = species?.BaseSpecies ?? set.Species;
            
            speciesCounts.TryGetValue(baseSpeciesId, out int count);
            speciesCounts[baseSpeciesId] = count + 1;
        }

        foreach (var (speciesId, count) in speciesCounts)
        {
            if (count > 1)
            {
                var speciesName = _library.Species.TryGetValue(speciesId, out var s) ? s.Name : speciesId.ToString();
                problems.Add($"Species Clause: You are limited to one of each Pokemon species. {speciesName} appears {count} times.");
            }
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
        bool canLearn = false;
        string? levelProblem = null;

        foreach (var source in sources)
        {
            // Only consider Gen 9 sources for VGC
            if (source.Generation != Gen9)
            {
                continue;
            }

            // For level-up moves, check if Pokemon is high enough level
            if (source.SourceType == MoveSourceType.LevelUp && source.LevelOrIndex.HasValue)
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

public class PokemonSources
{

}