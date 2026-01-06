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
        List<Pokemon> pokemonList = [];
        foreach (Side side in Sides)
        {
            pokemonList.AddRange(side.Active
                .Where(pokemon => pokemon != null && (includeFainted || !pokemon.Fainted))
                .Select(pokemon => pokemon!));
        }

        return pokemonList;
    }

    public int CanSwitch(Side side)
    {
        return PossibleSwitches(side).Count;
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