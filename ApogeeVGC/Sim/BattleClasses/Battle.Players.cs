using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class Battle
{
    /// <summary>
    /// Gets a Pokemon by its full name string.
    /// Searches through all sides and returns the first Pokemon with a matching fullname.
    /// </summary>
    /// <param name="fullname">The full name of the Pokemon to find</param>
    /// <returns>The Pokemon if found, otherwise null</returns>
    public Pokemon? GetPokemon(string fullname)
    {
        return Sides.SelectMany(side =>
            side.Pokemon.Where(pokemon => pokemon.Fullname == fullname)).FirstOrDefault();
    }

    /// <summary>
    /// Gets a Pokemon by another Pokemon's full name.
    /// This overload extracts the fullname from the provided Pokemon and searches for a match.
    /// </summary>
    /// <param name="pokemon">The Pokemon whose fullname to search for</param>
    /// <returns>The Pokemon if found, otherwise null</returns>
    public Pokemon? GetPokemon(Pokemon pokemon)
    {
        return GetPokemon(pokemon.Fullname);
    }

    /// <summary>
    /// Gets all Pokemon from all sides in the battle.
    /// </summary>
    /// <returns>A list containing all Pokemon from both sides</returns>
    public List<Pokemon> GetAllPokemon()
    {
        List<Pokemon> pokemonList = [];
        foreach (Side side in Sides)
        {
            pokemonList.AddRange(side.Pokemon);
        }

        return pokemonList;
    }

    public List<Pokemon> GetAllActive(bool includeFainted = false)
    {
        var pokemonList = new List<Pokemon>(Sides.Count * 2);
        FillAllActive(pokemonList, includeFainted);
        return pokemonList;
    }

    /// <summary>
    /// Fills an existing list with all active, non-fainted Pokémon across all sides.
    /// The list is cleared before filling. Use with <see cref="RentPokemonList"/> to avoid allocations.
    /// </summary>
    internal void FillAllActive(List<Pokemon> list, bool includeFainted = false)
    {
        list.Clear();
        foreach (Side side in Sides)
        {
            foreach (Pokemon? pokemon in side.Active)
            {
                if (pokemon != null && (includeFainted || !pokemon.Fainted))
                {
                    list.Add(pokemon);
                }
            }
        }
    }

    /// <summary>
    /// Zero-allocation enumeration of all active, non-fainted Pokémon across all sides.
    /// Use this in <c>foreach</c> loops instead of <see cref="GetAllActive"/> when
    /// the caller does not need a materialized <see cref="List{T}"/>.
    /// </summary>
    public ActivePokemonEnumerable EnumerateAllActive(bool includeFainted = false)
    {
        return new ActivePokemonEnumerable(Sides, includeFainted);
    }

    /// <summary>
    /// Struct-based enumerable for active Pokémon. Allows duck-typed <c>foreach</c>
    /// with a <see cref="ActivePokemonEnumerator"/> that never allocates.
    /// </summary>
    public readonly struct ActivePokemonEnumerable(List<Side> sides, bool includeFainted)
    {
        public ActivePokemonEnumerator GetEnumerator() => new(sides, includeFainted);
    }

    /// <summary>
    /// Value-type enumerator over all active (optionally including fainted) Pokémon across sides.
    /// </summary>
    public struct ActivePokemonEnumerator(List<Side> sides, bool includeFainted)
    {
        private int _sideIndex;
        private int _activeIndex = -1;

        public Pokemon Current { get; private set; } = null!;

        public bool MoveNext()
        {
            while (_sideIndex < sides.Count)
            {
                List<Pokemon?> active = sides[_sideIndex].Active;
                _activeIndex++;
                while (_activeIndex < active.Count)
                {
                    Pokemon? pokemon = active[_activeIndex];
                    if (pokemon != null && (includeFainted || !pokemon.Fainted))
                    {
                        Current = pokemon;
                        return true;
                    }
                    _activeIndex++;
                }
                _sideIndex++;
                _activeIndex = -1;
            }
            return false;
        }
    }

    public int CanSwitch(Side side)
    {
        return CountPossibleSwitches(side);
    }

    /// <summary>
    /// Counts Pokémon that can be switched in without allocating a list.
    /// </summary>
    private static int CountPossibleSwitches(Side side)
    {
        if (side.PokemonLeft <= 0) return 0;

        int count = 0;
        for (int i = side.Active.Count; i < side.Pokemon.Count; i++)
        {
            if (!side.Pokemon[i].Fainted)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// Gets a random Pokémon that can be switched in for the given side.
    /// Returns null if no Pokémon are available to switch in.
    /// </summary>
    /// <param name="side">The side to get a random switchable Pokémon for</param>
    /// <returns>A random switchable Pokémon, or null if none available</returns>
    public Pokemon? GetRandomSwitchable(Side side)
    {
        var canSwitchIn = PossibleSwitches(side);
        return canSwitchIn.Count > 0 ? Sample(canSwitchIn) : null;
    }

    /// <summary>
    /// Gets all Pokémon that can be switched in for the given side.
    /// Only includes non-fainted Pokémon that are not currently active.
    /// </summary>
    /// <param name="side">The side to get possible switches for</param>
    /// <returns>A list of all Pokémon that can be switched in</returns>
    private static List<Pokemon> PossibleSwitches(Side side)
    {
        // No Pokémon left on the side
        if (side.PokemonLeft <= 0) return [];

        List<Pokemon> canSwitchIn = [];

        // Iterate through Pokemon starting after the active slots
        // Active Pokemon are at indices [0, side.Active.Count)
        // Bench Pokemon are at indices [side.Active.Count, side.Pokemon.Count)
        for (int i = side.Active.Count; i < side.Pokemon.Count; i++)
        {
            Pokemon pokemon = side.Pokemon[i];
            if (!pokemon.Fainted)
            {
                canSwitchIn.Add(pokemon);
            }
        }

        return canSwitchIn;
    }

    private Side GetSide(SideId id)
    {
        return id switch
        {
            SideId.P1 => Sides[0],
            SideId.P2 => Sides[1],
            _ => throw new ArgumentOutOfRangeException(nameof(id), $"Invalid Side: {id}"),
        };
    }
}