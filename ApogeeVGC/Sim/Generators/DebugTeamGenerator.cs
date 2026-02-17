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
/// Generates random Pokemon teams using only whitelisted elements from DebugElementPools.
/// Used for incremental debugging to identify which elements cause bugs.
/// </summary>
public class DebugTeamGenerator
{
    private const int MoveCount = 4;
    private const int MaxEvPerStat = 252;
    private const int Gen9 = 9;
    private const int DefaultEvLimit = 510;
    private const int DefaultTeamSize = 6;

    private readonly Library _library;
    private readonly DebugElementPools _pools;
    private readonly Random _random;
    private readonly int _teamSize;
    private readonly int _evLimit;

    // Filtered lists based on whitelist
    private readonly List<SpecieId> _availableSpecies;
    private readonly List<ItemId> _availableItems;

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
    /// Creates a new debug team generator with whitelisted elements.
    /// </summary>
    /// <param name="library">The game data library.</param>
    /// <param name="pools">The debug element pools containing whitelisted elements.</param>
    /// <param name="seed">Random seed for reproducibility.</param>
    /// <param name="teamSize">Team size (default 6).</param>
    /// <param name="evLimit">EV limit per Pokemon (default 510).</param>
    public DebugTeamGenerator(
        Library library,
        DebugElementPools pools,
        int seed,
        int teamSize = DefaultTeamSize,
        int evLimit = DefaultEvLimit)
    {
        ArgumentNullException.ThrowIfNull(library);
        ArgumentNullException.ThrowIfNull(pools);

        _library = library;
        _pools = pools;
        _random = new Random(seed);
        _teamSize = teamSize;
        _evLimit = evLimit;

        _availableSpecies = BuildAvailableSpeciesList();
        _availableItems = BuildAvailableItemsList();

        if (_availableSpecies.Count < _teamSize)
        {
            throw new InvalidOperationException(
                $"Not enough available species ({_availableSpecies.Count}) for team size ({_teamSize}). " +
                $"Add more species to the whitelist.");
        }

        if (_availableItems.Count < _teamSize)
        {
            throw new InvalidOperationException(
                $"Not enough available items ({_availableItems.Count}) for team size ({_teamSize}). " +
                $"Add more items to the whitelist.");
        }
    }

    /// <summary>
    /// Generates a random team using only whitelisted elements.
    /// </summary>
    public List<PokemonSet> GenerateTeam()
    {
        var team = new List<PokemonSet>(_teamSize);
        var usedSpecies = new HashSet<SpecieId>();
        var usedItems = new HashSet<ItemId>();

        for (var i = 0; i < _teamSize; i++)
        {
            var pokemon = GenerateRandomPokemon(usedSpecies, usedItems);
            team.Add(pokemon);
        }

        return team;
    }

    /// <summary>
    /// Generates a single random Pokemon using whitelisted elements.
    /// </summary>
    private PokemonSet GenerateRandomPokemon(HashSet<SpecieId> usedSpecies, HashSet<ItemId> usedItems)
    {
        var speciesId = PickRandomUnused(_availableSpecies, usedSpecies);
        usedSpecies.Add(speciesId);
        var species = _library.Species[speciesId];

        // Pick a random ability from the species' available abilities (filtered by whitelist)
        var ability = PickRandomAbility(species);

        // Pick 4 random moves from the learnset (filtered by whitelist)
        var (moves, requiredLevel) = PickRandomMoves(speciesId);

        // Pick a random item that hasn't been used yet
        var item = PickRandomUnused(_availableItems, usedItems);
        usedItems.Add(item);

        // Pick a random nature
        var nature = PickRandomNature();

        // Generate random EVs
        var evs = GenerateRandomEvs();

        // Pick a random tera type
        var teraType = TeraTypes[_random.Next(TeraTypes.Length)];

        // Determine gender based on species
        var gender = DetermineGender(species);

        // Set level to the maximum of required level for moves and 50 (VGC minimum)
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
    /// Builds the list of species available for generation (respecting whitelist).
    /// </summary>
    private List<SpecieId> BuildAvailableSpeciesList()
    {
        var available = new List<SpecieId>();
        var allowedSpecies = _pools.AvailableSpecies;

        foreach (var speciesId in allowedSpecies)
        {
            if (!_library.Species.ContainsKey(speciesId))
            {
                continue;
            }

            var species = _library.Species[speciesId];

            // Skip battle-only formes
            if (species.BattleOnly.HasValue)
            {
                continue;
            }

            // Check if the species has at least MoveCount moves available from the whitelist
            if (!HasEnoughMoves(speciesId))
            {
                continue;
            }

            // Check if the species has at least one ability from the whitelist
            if (!HasAvailableAbility(species))
            {
                continue;
            }

            available.Add(speciesId);
        }

        return available;
    }

    /// <summary>
    /// Checks if the species has enough moves available from the whitelist.
    /// </summary>
    private bool HasEnoughMoves(SpecieId speciesId)
    {
        if (!_library.Learnsets.TryGetValue(speciesId, out var learnset) ||
            learnset.LearnsetData == null)
        {
            return false;
        }

        var availableMoves = _pools.AvailableMoves;
        var availableMoveCount = learnset.LearnsetData
            .Count(kvp => availableMoves.Contains(kvp.Key) &&
                          kvp.Value.Any(source => source.Generation == Gen9));

        return availableMoveCount >= MoveCount;
    }

    /// <summary>
    /// Checks if the species has at least one ability from the whitelist.
    /// </summary>
    private bool HasAvailableAbility(Species species)
    {
        var allowedAbilities = _pools.AvailableAbilities;

        if (species.Abilities.Slot0 != AbilityId.None && allowedAbilities.Contains(species.Abilities.Slot0))
        {
            return true;
        }

        if (species.Abilities.Slot1 is { } slot1 && slot1 != AbilityId.None && allowedAbilities.Contains(slot1))
        {
            return true;
        }

        if (species.Abilities.Hidden is { } hidden && hidden != AbilityId.None && allowedAbilities.Contains(hidden))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Builds the list of items available for generation (respecting whitelist).
    /// </summary>
    private List<ItemId> BuildAvailableItemsList()
    {
        var available = new List<ItemId>();
        var allowedItems = _pools.AvailableItems;

        foreach (var itemId in allowedItems)
        {
            if (itemId != ItemId.None && _library.Items.ContainsKey(itemId))
            {
                available.Add(itemId);
            }
        }

        return available;
    }

    /// <summary>
    /// Picks a random element from the list that isn't in the used set.
    /// </summary>
    private T PickRandomUnused<T>(List<T> available, HashSet<T> used) where T : notnull
    {
        var unused = available.Where(x => !used.Contains(x)).ToList();

        if (unused.Count == 0)
        {
            throw new InvalidOperationException("No unused elements available to pick from.");
        }

        return unused[_random.Next(unused.Count)];
    }

    /// <summary>
    /// Picks a random valid ability for the species (from whitelist).
    /// </summary>
    private AbilityId PickRandomAbility(Species species)
    {
        var abilities = new List<AbilityId>();
        var allowedAbilities = _pools.AvailableAbilities;

        if (species.Abilities.Slot0 != AbilityId.None && allowedAbilities.Contains(species.Abilities.Slot0))
        {
            abilities.Add(species.Abilities.Slot0);
        }

        if (species.Abilities.Slot1 is { } slot1 && slot1 != AbilityId.None && allowedAbilities.Contains(slot1))
        {
            abilities.Add(slot1);
        }

        if (species.Abilities.Hidden is { } hidden && hidden != AbilityId.None && allowedAbilities.Contains(hidden))
        {
            abilities.Add(hidden);
        }

        if (abilities.Count == 0)
        {
            throw new InvalidOperationException($"Species {species.Name} has no whitelisted abilities.");
        }

        return abilities[_random.Next(abilities.Count)];
    }

    /// <summary>
    /// Picks 4 random moves from the Pokemon's learnset (from whitelist).
    /// </summary>
    private (IReadOnlyList<MoveId> Moves, int RequiredLevel) PickRandomMoves(SpecieId speciesId)
    {
        if (!_library.Learnsets.TryGetValue(speciesId, out var learnset) ||
            learnset.LearnsetData == null)
        {
            throw new InvalidOperationException($"No learnset found for {speciesId}.");
        }

        var allowedMoves = _pools.AvailableMoves;

        // Filter to only moves that are whitelisted and have a Gen 9 source
        var availableMoves = learnset.LearnsetData
            .Where(kvp => allowedMoves.Contains(kvp.Key) &&
                          kvp.Value.Any(source => source.Generation == Gen9))
            .Select(kvp => kvp.Key)
            .ToList();

        if (availableMoves.Count < MoveCount)
        {
            throw new InvalidOperationException(
                $"Not enough whitelisted Gen 9 moves available for {speciesId}. " +
                $"Has {availableMoves.Count}, needs {MoveCount}.");
        }

        // Shuffle and take first 4
        var selectedMoves = new List<MoveId>();
        var indices = Enumerable.Range(0, availableMoves.Count).ToList();

        for (var i = 0; i < MoveCount; i++)
        {
            var indexPos = _random.Next(indices.Count);
            var moveIndex = indices[indexPos];
            indices.RemoveAt(indexPos);
            selectedMoves.Add(availableMoves[moveIndex]);
        }

        // Calculate the minimum level required
        var requiredLevel = 1;
        foreach (var moveId in selectedMoves)
        {
            if (learnset.LearnsetData.TryGetValue(moveId, out var sources))
            {
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
    /// Picks a random nature.
    /// </summary>
    private Nature PickRandomNature()
    {
        var natures = _library.Natures.Values.ToList();
        return natures[_random.Next(natures.Count)];
    }

    /// <summary>
    /// Generates random EVs respecting limits.
    /// </summary>
    private StatsTable GenerateRandomEvs()
    {
        var stats = new int[6];
        var remaining = _evLimit;

        // Distribute EVs randomly across stats
        while (remaining > 0)
        {
            var statIndex = _random.Next(6);
            var maxForStat = Math.Min(remaining, MaxEvPerStat - stats[statIndex]);

            if (maxForStat <= 0)
            {
                continue;
            }

            // Add EVs in multiples of 4 (how EVs actually work)
            var toAdd = (_random.Next(maxForStat / 4 + 1)) * 4;
            toAdd = Math.Min(toAdd, maxForStat);
            stats[statIndex] += toAdd;
            remaining -= toAdd;

            // Sometimes stop early for variety
            if (_random.Next(10) == 0)
            {
                break;
            }
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
        var ratio = species.GenderRatio;

        if (ratio.M == 0 && ratio.F == 0)
        {
            return GenderId.N; // Genderless
        }

        if (ratio.M == 0)
        {
            return GenderId.F; // Female only
        }

        if (ratio.F == 0)
        {
            return GenderId.M; // Male only
        }

        // Random based on ratio
        var roll = _random.NextDouble();
        return roll < ratio.M ? GenderId.M : GenderId.F;
    }
}
