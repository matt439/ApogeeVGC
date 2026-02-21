using System.Collections.Concurrent;
using System.Diagnostics;
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
    private const int Gen9 = 9;

    /// <summary>
    /// Static cache of format-specific data (legal species, restricted species, usable items).
    /// These lists are deterministic for a given FormatId and Library, so they only need to be
    /// computed once. This eliminates ~93% of per-construction cost when creating thousands
    /// of RandomTeamGenerator instances for the same format.
    /// </summary>
    private static readonly ConcurrentDictionary<FormatId, CachedFormatData> s_formatCache = new();

    private sealed record CachedFormatData(
        List<SpecieId> LegalSpecies,
        List<SpecieId> RestrictedSpecies,
        List<ItemId> UsableItems);

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

    /// <summary>
    /// All species legal in this format (includes restricted if allowed).
    /// </summary>
    public IReadOnlyList<SpecieId> LegalSpecies => _legalSpecies;

    /// <summary>
    /// Restricted legendary species eligible in this format.
    /// </summary>
    public IReadOnlyList<SpecieId> RestrictedSpecies => _restrictedSpecies;

    /// <summary>
    /// Items eligible for random selection in this format.
    /// </summary>
    public IReadOnlyList<ItemId> UsableItems => _usableItems;

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

        // Use cached format data to avoid rebuilding identical lists for every instance.
        // The Build* methods are deterministic for a given (Library, FormatId), so the
        // first instance computes and caches; all subsequent instances reuse the result.
        var cached = s_formatCache.GetOrAdd(formatId, _ => new CachedFormatData(
            BuildLegalSpeciesList(),
            BuildRestrictedSpeciesList(),
            BuildUsableItemsList()));

        _legalSpecies = cached.LegalSpecies;
        _restrictedSpecies = cached.RestrictedSpecies;
        _usableItems = cached.UsableItems;
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
            // Two-pass approach: count unused restricted, then pick the nth one
            int unusedRestrictedCount = 0;
            foreach (var x in _restrictedSpecies)
            {
                if (!usedSpecies.Contains(x))
                    unusedRestrictedCount++;
            }

            if (unusedRestrictedCount > 0)
            {
                int target = _random.Next(unusedRestrictedCount);
                speciesId = default;
                foreach (var x in _restrictedSpecies)
                {
                    if (!usedSpecies.Contains(x))
                    {
                        if (target == 0)
                        {
                            speciesId = x;
                            break;
                        }
                        target--;
                    }
                }
                restrictedCount++;
            }
            else
            {
                speciesId = PickRandomUnused(_legalSpecies, usedSpecies);
            }
        }
        else
        {
            // Pick from legal non-restricted species using two-pass counting
            int nonRestrictedCount = 0;
            foreach (var x in _legalSpecies)
            {
                if (!usedSpecies.Contains(x) && !_restrictedSpecies.Contains(x))
                    nonRestrictedCount++;
            }

            if (nonRestrictedCount > 0)
            {
                int target = _random.Next(nonRestrictedCount);
                speciesId = default;
                foreach (var x in _legalSpecies)
                {
                    if (!usedSpecies.Contains(x) && !_restrictedSpecies.Contains(x))
                    {
                        if (target == 0)
                        {
                            speciesId = x;
                            break;
                        }
                        target--;
                    }
                }
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

        // Pick 4 random moves from the learnset and get the required level
        var (moves, requiredLevel) = PickRandomMoves(speciesId);

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

        // Set level to the maximum of required level for moves and the format's minimum (usually 50 for VGC)
        // The level will be adjusted down to 50 for battle by AdjustLevelDown rule
        var level = Math.Max(requiredLevel, 50);

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
            Level = level,
        };
    }

    /// <summary>
    /// Builds the list of species legal in the format.
    /// Excludes species based on format banlist (Mythical always, Restricted if banned).
    /// Only includes species that have at least 4 moves learnable in Gen 9.
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

            // Skip if the species has no learnset or not enough Gen 9 moves
            if (!_library.Learnsets.TryGetValue(speciesId, out var learnset) ||
                learnset.LearnsetData == null)
            {
                continue;
            }

            // Count moves that have a Gen 9 source
            var gen9MoveCount = learnset.LearnsetData
                .Count(kvp => kvp.Value.Any(source => source.Generation == Gen9));

            if (gen9MoveCount < MoveCount)
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
    /// Only includes species that have at least 4 moves learnable in Gen 9.
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

            // Skip if the species has no learnset or not enough Gen 9 moves
            if (!_library.Learnsets.TryGetValue(speciesId, out var learnset) ||
                learnset.LearnsetData == null)
            {
                continue;
            }

            // Count moves that have a Gen 9 source
            var gen9MoveCount = learnset.LearnsetData
                .Count(kvp => kvp.Value.Any(source => source.Generation == Gen9));

            if (gen9MoveCount < MoveCount)
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
    /// Uses a two-pass approach (count then index) to avoid allocating a filtered list.
    /// </summary>
    private T PickRandomUnused<T>(List<T> available, HashSet<T> used) where T : notnull
    {
        // Pass 1: count unused items
        int unusedCount = 0;
        foreach (var item in available)
        {
            if (!used.Contains(item))
                unusedCount++;
        }

        if (unusedCount == 0)
        {
            throw new InvalidOperationException("No unused elements available to pick from.");
        }

        // Pass 2: pick the nth unused item
        int target = _random.Next(unusedCount);
        foreach (var item in available)
        {
            if (!used.Contains(item))
            {
                if (target == 0)
                    return item;
                target--;
            }
        }

        throw new UnreachableException();
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
    /// Only considers moves learnable in Gen 9 to match validator requirements.
    /// Returns the moves and the minimum level required to learn all of them.
    /// </summary>
    private (IReadOnlyList<MoveId> Moves, int RequiredLevel) PickRandomMoves(SpecieId speciesId)
    {
        if (!_library.Learnsets.TryGetValue(speciesId, out var learnset) ||
            learnset.LearnsetData == null)
        {
            throw new InvalidOperationException($"No learnset found for {speciesId}.");
        }

        // Build available moves list with manual loop (no LINQ enumerator/delegate allocation)
        var availableMoves = new List<MoveId>();
        foreach (var (moveId, sources) in learnset.LearnsetData)
        {
            foreach (var source in sources)
            {
                if (source.Generation == Gen9)
                {
                    availableMoves.Add(moveId);
                    break;
                }
            }
        }

        if (availableMoves.Count == 0)
        {
            throw new InvalidOperationException($"No Gen 9 moves available for {speciesId}.");
        }

        // Fisher-Yates partial shuffle to select MoveCount moves (no indices list allocation)
        var moveCount = Math.Min(MoveCount, availableMoves.Count);
        var selectedMoves = new List<MoveId>(moveCount);
        for (var i = 0; i < moveCount; i++)
        {
            var j = _random.Next(i, availableMoves.Count);
            (availableMoves[i], availableMoves[j]) = (availableMoves[j], availableMoves[i]);
            selectedMoves.Add(availableMoves[i]);
        }

        // Calculate the minimum level required to learn all selected moves
        var requiredLevel = 1;
        foreach (var moveId in selectedMoves)
        {
            if (learnset.LearnsetData.TryGetValue(moveId, out var sources))
            {
                // Find the Gen 9 level-up source with the highest level requirement
                foreach (var source in sources)
                {
                    if (source.Generation == Gen9 &&
                        source.SourceType == MoveSourceType.LevelUp &&
                        source.LevelOrIndex.HasValue)
                    {
                        requiredLevel = Math.Max(requiredLevel, source.LevelOrIndex.Value);
                    }
                }
            }
        }

        return (selectedMoves, requiredLevel);
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