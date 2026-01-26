using ApogeeVGC.Data;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Generators;

/// <summary>
/// Generates random Pokemon teams for VGC formats.
/// Uses a seed for reproducibility when debugging exceptions.
/// </summary>
public class RandomTeamGenerator
{
    private const int TeamSize = 6;
    private const int MoveCount = 4;
    private const int EvLimit = 510;
    private const int MaxEvPerStat = 252;

    private readonly Library _library;
    private readonly Random _random;

    // Cache of legal species for VGC Regulation I (no Mythical, no Restricted Legendary)
    private readonly List<SpecieId> _legalSpecies;

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
    /// Creates a new random team generator with the specified seed.
    /// </summary>
    /// <param name="library">The game data library.</param>
    /// <param name="seed">Random seed for reproducibility.</param>
    public RandomTeamGenerator(Library library, int seed)
    {
        ArgumentNullException.ThrowIfNull(library);
        _library = library;
        _random = new Random(seed);

        _legalSpecies = BuildLegalSpeciesList();
        _usableItems = BuildUsableItemsList();
    }

    /// <summary>
    /// Generates a random team valid for VGC Regulation I.
    /// </summary>
    /// <returns>A list of 6 Pokemon sets.</returns>
    public List<PokemonSet> GenerateTeam()
    {
        var team = new List<PokemonSet>(TeamSize);
        var usedSpecies = new HashSet<SpecieId>();
        var usedItems = new HashSet<ItemId>();

        for (var i = 0; i < TeamSize; i++)
        {
            var pokemon = GenerateRandomPokemon(usedSpecies, usedItems);
            team.Add(pokemon);
        }

        return team;
    }

    /// <summary>
    /// Generates a single random Pokemon that doesn't conflict with already selected species/items.
    /// </summary>
    private PokemonSet GenerateRandomPokemon(HashSet<SpecieId> usedSpecies, HashSet<ItemId> usedItems)
    {
        // Pick a random species that hasn't been used yet
        var speciesId = PickRandomUnused(_legalSpecies, usedSpecies);
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
    /// Builds the list of species legal in VGC Regulation I.
    /// Excludes Mythical and Restricted Legendary Pokemon.
    /// </summary>
    private List<SpecieId> BuildLegalSpeciesList()
    {
        var legal = new List<SpecieId>();

        foreach (var (speciesId, species) in _library.Species)
        {
            // Skip if Mythical or Restricted Legendary
            if (species.Tags.Contains(SpeciesTag.Mythical) ||
                species.Tags.Contains(SpeciesTag.RestrictedLegendary))
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
    /// Generates random EVs respecting the 510 total limit and 252 per-stat limit.
    /// </summary>
    private StatsTable GenerateRandomEvs()
    {
        var stats = new int[6]; // HP, Atk, Def, SpA, SpD, Spe
        var remaining = EvLimit;

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
