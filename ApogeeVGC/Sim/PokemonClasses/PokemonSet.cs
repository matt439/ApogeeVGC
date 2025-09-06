namespace ApogeeVGC.Sim.PokemonClasses;

public class PokemonSet
{
    public required Pokemon[] Pokemons
    {
        get;
        set
        {
            if (value == null || value.Length == 0)
            {
                throw new ArgumentException("Pokemon set must contain at least one Pokemon.");
            }
            if (value.Length > 6)
            {
                throw new ArgumentException("Pokemon set cannot contain more than 6 Pokemon.");
            }
            field = value;
        }
    }
    public int PokemonCount => Pokemons.Length;
    public Pokemon[] AlivePokemon => Pokemons.Where(pokemon => !pokemon.IsFainted).ToArray();
    public int AlivePokemonCount => AlivePokemon.Length;
    public int FaintedCount => PokemonCount - AlivePokemonCount;
    public bool AllFainted => AlivePokemonCount == 0;
    public bool AnyTeraUsed => Pokemons.Any(pokemon => pokemon.IsTeraUsed);

    public Pokemon[] UnusedPokemons { get; set; } = [];

    /// /// <summary>
    /// Creates a deep copy of this PokemonSet for MCTS simulation purposes.
    /// </summary>
    /// <returns>A new PokemonSet instance with copied Pokemon</returns>
    public PokemonSet Copy()
    {
        return new PokemonSet
        {
            Pokemons = Pokemons.Select(pokemon => pokemon.Copy()).ToArray(),
        };
    }
}