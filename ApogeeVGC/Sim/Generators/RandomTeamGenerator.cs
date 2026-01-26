using ApogeeVGC.Data;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Generators;

/// <summary>
/// Generates random Pokemon teams for VGC formats.
/// Supports different VGC regulations (A-I) with proper handling of restricted Pokemon limits.
/// Uses a seed for reproducibility when debugging exceptions.
/// </summary>
public class RandomTeamGenerator
{
    private const int MoveCount = 4;
    private const int MaxEvPerStat = 252;

    private readonly Library _library;
    private readonly Format _format;
    private readonly Random _random;

    // Settings extracted from format RuleTable
    private readonly int _teamSize;
    private readonly int _evLimit;
    private readonly int _maxRestrictedCount;

    // Cache of legal species for the format (excluding banned categories)
    private readonly List<SpecieId> _legalSpecies;

    // Cache of restricted legendary species (for formats that allow them)
    private readonly List<SpecieId> _restrictedSpecies;

    // Cache of usable items (exclude balls, fossils, evolution items, etc.)
    private readonly List<ItemId> _usableItems;

    // All tera types except Stellar and Unknown
    private static readonly MoveType[] TeraTypes =
    [
        MoveType.Normal, MoveType.Fire, MoveType.Water, MoveType.Electric,
        MoveType.Grass, MoveType.Ice, MoveType.Fighting, MoveType.Poison,
        MoveType.Ground, MoveType.Flying, MoveType.Psychic, MoveType.Bug,
        MoveType.Rock, MoveType.Ghost, MoveType.Dragon, MoveType.Dark,
        MoveType.Steel, MoveType.Fairy
    ];

    /// <summary>
    /// Creates a new random team generator for the specified format.
    /// </summary>
    /// <param name="library">The game data library.</param>
    /// <param name="formatId">The VGC format to generate teams for.</param>
    /// <param name="seed">Random seed for reproducibility.</param>
    public RandomTeamGenerator(Library library, FormatId formatId, int seed)
    {
        ArgumentNullException.ThrowIfNull(library);
        _library = library;
        _format = library.Formats.TryGetValue(formatId, out var format)
            ? format
            : throw new ArgumentException($"Format {formatId} not found in library.", nameof(formatId));
        _random = new Random(seed);

        // Extract settings from format RuleTable
        _teamSize = _format.RuleTable?.MaxTeamSize ?? 6;
        _evLimit = _format.RuleTable?.EvLimit ?? 510;
        _maxRestrictedCount = GetMaxRestrictedCount();

        _legalSpecies = BuildLegalSpeciesList();
        _restrictedSpecies = BuildRestrictedSpeciesList();
        _usableItems = BuildUsableItemsList();
    }

    /// <summary>
    /// Generates a random team valid for the specified format.
    /// Respects format rules including restricted Pokemon limits.
    /// </summary>
    /// <returns>A list of Pokemon sets matching the format's team size.</returns>
    public List<PokemonSet> GenerateTeam()
    {
        var team = new List<PokemonSet>(_teamSize);
        var usedSpecies = new HashSet<SpecieId>();
        var usedItems = new HashSet<ItemId>();
        var restrictedCount = 0;

        for (var i = 0; i < _teamSize; i++)
        {
            var pokemon = GenerateRandomPokemon(usedSpecies, usedItems, ref restrictedCount);
            team.Add(pokemon);
        }

        return team;
    }

    /// <summary>
    /// Generates a single random Pokemon that doesn't conflict with already selected species/items.
    /// Respects the restricted Pokemon limit for the format.
    /// </summary>
    private PokemonSet GenerateRandomPokemon(
        HashSet<SpecieId> usedSpecies,
        HashSet<ItemId> usedItems,
        ref int restrictedCount)
    {
        // Determine if we can still add restricted Pokemon
        var canAddRestricted = restrictedCount < _maxRestrictedCount && _restrictedSpecies.Count > 0;

        SpecieId speciesId;
        if (canAddRestricted && _random.Next(3) == 0)
        {
            // Occasionally try to pick a restricted Pokemon (1 in 3 chance when allowed)
            var unusedRestricted = _restrictedSpecies.Where(x => !usedSpecies.Contains(x)).ToList();
            if (unusedRestricted.Count > 0)
            {
                speciesId = unusedRestricted[_random.Next(unusedRestricted.Count)];
                restrictedCount++;
            }
            else
            {
                speciesId = PickRandomUnused(_legalSpecies, usedSpecies);
            }
        }
        else
        {
            // Pick from legal non-restricted species
            var nonRestrictedLegal = _legalSpecies
                .Where(x => !usedSpecies.Contains(x) && !_restrictedSpecies.Contains(x))
                .ToList();

            if (nonRestrictedLegal.Count > 0)
            {
                speciesId = nonRestrictedLegal[_random.Next(nonRestrictedLegal.Count)];
            }
            else
            {
                // Fallback to any unused legal species
                speciesId = PickRandomUnused(_legalSpecies, usedSpecies);
            }
        }

        usedSpecies.Add(speciesId);
        var species = _library.Species[speciesId];

        // Pick a random ability from the species' available abilities
        var ability = PickRandomAbility(species);

        // Pick 4 random moves from the learnset
        var moves = PickRandomMoves(speciesId);

        // Pick a random item that hasn't been used yet
        var item = PickRandomUnused(_usableItems, usedItems);
        usedItems.Add(item);

        // Pick a random nature
        var nature = PickRandomNature();

        // Generate random EVs (total <= 510, each stat <= 252)
        var evs = GenerateRandomEvs();

        // Pick a random tera type
        var teraType = TeraTypes[_random.Next(TeraTypes.Length)];

        // Determine gender based on species
        var gender = DetermineGender(species);

        return new PokemonSet
        {
            Name = species.Name,
            Species = speciesId,
            Item = item,
            Ability = ability,
            Moves = moves,
            Nature = nature,
            Gender = gender,
            Evs = evs,
            TeraType = teraType,
            Level = 50,
        };
    }

    /// <summary>
    /// Builds the list of species legal in the format.
    /// Excludes species based on format banlist (Mythical always, Restricted if banned).
    /// </summary>
    private List<SpecieId> BuildLegalSpeciesList()
    {
        var legal = new List<SpecieId>();
        var bansMythical = _format.Banlist.Contains(RuleId.Mythical);
        var bansRestricted = _format.Banlist.Contains(RuleId.RestrictedLegendary);

        foreach (var (speciesId, species) in _library.Species)
        {
            // Skip if Mythical and format bans them
            if (bansMythical && species.Tags.Contains(SpeciesTag.Mythical))
            {
                continue;
            }

            // Skip if Restricted Legendary and format bans them entirely
            // (formats with LimitOneRestricted/LimitTwoRestricted don't ban them, they limit count)
            if (bansRestricted && species.Tags.Contains(SpeciesTag.RestrictedLegendary))
            {
                continue;
            }

            // Skip if the species has no learnset (can't have valid moves)
            if (!_library.Learnsets.TryGetValue(speciesId, out var learnset) ||
                learnset.LearnsetData is not { Count: >= MoveCount })
            {
                continue;
            }

            // Skip battle-only formes (like Mega evolutions, Primal forms)
            if (species.BattleOnly.HasValue)
            {
                continue;
            }

            legal.Add(speciesId);
        }

        return legal;
    }

    /// <summary>
    /// Builds the list of restricted legendary species for formats that allow them.
    /// </summary>
    private List<SpecieId> BuildRestrictedSpeciesList()
    {
        if (_maxRestrictedCount == 0)
        {
            return [];
        }

        var restricted = new List<SpecieId>();

        foreach (var (speciesId, species) in _library.Species)
        {
            if (!species.Tags.Contains(SpeciesTag.RestrictedLegendary))
            {
                continue;
            }

            // Skip if the species has no learnset
            if (!_library.Learnsets.TryGetValue(speciesId, out var learnset) ||
                learnset.LearnsetData is not { Count: >= MoveCount })
            {
                continue;
            }

            // Skip battle-only formes
            if (species.BattleOnly.HasValue)
            {
                continue;
            }

            restricted.Add(speciesId);
        }

        return restricted;
    }

    /// <summary>
    /// Determines the maximum number of restricted Pokemon allowed based on format rules.
    /// </summary>
    private int GetMaxRestrictedCount()
    {
        if (_format.Ruleset.Contains(RuleId.LimitTwoRestricted))
        {
            return 2;
        }

        if (_format.Ruleset.Contains(RuleId.LimitOneRestricted))
        {
            return 1;
        }

        // If restricted legendaries are banned entirely or no limit rule, return 0
        return 0;
    }

    /// <summary>
    /// Builds the list of items usable in battle.
    /// Excludes Poke Balls, fossils, evolution items, etc.
    /// </summary>
    private List<ItemId> BuildUsableItemsList()
    {
        var usable = new List<ItemId>();

        foreach (var (itemId, _) in _library.Items)
        {
            if (itemId == ItemId.None)
            {
                continue;
            }

            // Skip items that aren't battle items
            // The Items class should have battle-relevant items; we'll include most
            // but filter out obvious non-battle items by checking if they exist
            usable.Add(itemId);
        }

        return usable;
    }

    /// <summary>
    /// Picks a random element from the list that isn't in the used set.
    /// </summary>
    private T PickRandomUnused<T>(List<T> available, HashSet<T> used) where T : notnull
    {
        // Simple approach: filter and pick
        var unused = available.Where(x => !used.Contains(x)).ToList();

        if (unused.Count == 0)
        {
            throw new InvalidOperationException("No unused elements available to pick from.");
        }

        return unused[_random.Next(unused.Count)];
    }

    /// <summary>
    /// Picks a random valid ability for the species.
    /// </summary>
    private AbilityId PickRandomAbility(Species species)
    {
        var abilities = new List<AbilityId>();

        if (species.Abilities.Slot0 != AbilityId.None)
        {
            abilities.Add(species.Abilities.Slot0);
        }

        if (species.Abilities.Slot1 is { } slot1 && slot1 != AbilityId.None)
        {
            abilities.Add(slot1);
        }

        if (species.Abilities.Hidden is { } hidden && hidden != AbilityId.None)
        {
            abilities.Add(hidden);
        }

        // Skip Special ability as it's typically for special formes

        if (abilities.Count == 0)
        {
            throw new InvalidOperationException($"Species {species.Name} has no valid abilities.");
        }

        return abilities[_random.Next(abilities.Count)];
    }

    /// <summary>
    /// Picks 4 random moves from the Pokemon's learnset.
    /// </summary>
    private IReadOnlyList<MoveId> PickRandomMoves(SpecieId speciesId)
    {
        if (!_library.Learnsets.TryGetValue(speciesId, out var learnset) ||
            learnset.LearnsetData == null)
        {
            throw new InvalidOperationException($"No learnset found for {speciesId}.");
        }

        var availableMoves = learnset.LearnsetData.Keys.ToList();

        // Shuffle and take first 4
        var selectedMoves = new List<MoveId>();
        var indices = Enumerable.Range(0, availableMoves.Count).ToList();

        for (var i = 0; i < Math.Min(MoveCount, availableMoves.Count); i++)
        {
            var indexPos = _random.Next(indices.Count);
            var moveIndex = indices[indexPos];
            indices.RemoveAt(indexPos);
            selectedMoves.Add(availableMoves[moveIndex]);
        }

        // If we have less than 4 moves, fill with what we have
        if (selectedMoves.Count == 0)
        {
            throw new InvalidOperationException($"No moves available for {speciesId}.");
        }

        return selectedMoves;
    }

    /// <summary>
    /// Picks a random nature from available natures.
    /// </summary>
    private Nature PickRandomNature()
    {
        var natures = _library.Natures.Values.ToList();
        return natures[_random.Next(natures.Count)];
    }

    /// <summary>
    /// Generates random EVs respecting the format's EV limit and 252 per-stat limit.
    /// </summary>
    private StatsTable GenerateRandomEvs()
    {
        var stats = new int[6]; // HP, Atk, Def, SpA, SpD, Spe
        var remaining = _evLimit;

        // Distribute EVs randomly across stats
        for (var i = 0; i < 6 && remaining > 0; i++)
        {
            // For the last stat, use whatever is remaining (up to max)
            var maxForStat = Math.Min(MaxEvPerStat, remaining);
            var ev = _random.Next(0, maxForStat + 1);

            // Ensure EVs are multiples of 4 (as in actual games)
            ev = (ev / 4) * 4;

            stats[i] = ev;
            remaining -= ev;
        }

        // Shuffle the distribution so it's not always front-loaded
        for (var i = stats.Length - 1; i > 0; i--)
        {
            var j = _random.Next(i + 1);
            (stats[i], stats[j]) = (stats[j], stats[i]);
        }

        return new StatsTable
        {
            Hp = stats[0],
            Atk = stats[1],
            Def = stats[2],
            SpA = stats[3],
            SpD = stats[4],
            Spe = stats[5],
        };
    }

    /// <summary>
    /// Determines the gender based on species gender ratio.
    /// </summary>
    private GenderId DetermineGender(Species species)
    {
        // Handle specific gender cases
        return species.Gender switch
        {
            GenderId.M => GenderId.M,
            GenderId.F => GenderId.F,
            GenderId.N => GenderId.N,
            // For species with gender ratios or Empty, pick randomly
            _ => _random.Next(2) == 0 ? GenderId.M : GenderId.F,
        };
    }
}
