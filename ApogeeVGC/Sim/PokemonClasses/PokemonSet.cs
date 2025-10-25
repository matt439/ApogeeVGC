using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.PokemonClasses;

public record PokemonSet
{
    public required string Name { get; init; }
    public SpecieId Species { get; init; }
    public ItemId Item { get; init; }
    public AbilityId Ability { get; init; }
    public required IReadOnlyList<MoveId> Moves
    {
        get;
        init
        {
            if (value == null || value.Count == 0)
            {
                throw new ArgumentException("Pokemon must have at least one move.");
            }
            if (value.Count > 4)
            {
                throw new ArgumentException("Pokemon cannot have more than 4 moves.");
            }
            // Check that all moves are unique
            if (value.Distinct().Count() != value.Count)
            {
                throw new ArgumentException("Pokemon cannot have duplicate moves.");
            }
            field = value;
        }
    }
    public required Nature Nature { get; init; }
    public GenderId Gender { get; init; }
    public required StatsTable Evs { get; init; }
    public StatsTable Ivs { get; init; } = StatsTable.PerfectIvs;
    public int Level
    {
        get;
        init
        {
            if (value is < 1 or > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Level must be between 1 and 100.");
            }

            field = value;
        }
    } = 50;
    public bool Shiny { get; init; }
    public int Happiness
    {
        get;
        init
        {
            if (value is < 0 or > 255)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Happiness must be between 0 and 255.");
            }
            field = value;
        }
    }
    public PokeballId Pokeball { get; init; } = PokeballId.Pokeball;
    public MoveType TeraType { get; init; }
}



///// <summary>
///// The set of Pokémon a player has in a battle.
///// The Pokemon in the set are not modified during battle.
///// Instead, Pokemon which will battle are copied into the Side's slots.
///// In a doubles battle, the 2 unused Pokemon can be accessed from the set.
///// </summary>
//public record PokemonSet
//{
//    public required IReadOnlyList<Pokemon> Pokemons
//    {
//        get;
//        init
//        {
//            if (value == null || value.Count == 0)
//            {
//                throw new ArgumentException("Pokemon set must contain at least one Pokemon.");
//            }
//            if (value.Count > 6)
//            {
//                throw new ArgumentException("Pokemon set cannot contain more than 6 Pokemon.");
//            }
//            field = value;
//        }
//    }
//    public int PokemonCount => Pokemons.Count;
//    //public Pokemon[] AlivePokemon => Pokemons.Where(pokemon => !pokemon.IsFainted).ToArray();
//    //public int AlivePokemonCount => AlivePokemon.Length;
//    //public int FaintedCount => PokemonCount - AlivePokemonCount;
//    //public bool AllFainted => AlivePokemonCount == 0;
//    //public bool AnyTeraUsed => Pokemons.Any(pokemon => pokemon.IsTeraUsed);
//    public required Side Side { get; init; }

//    /// /// <summary>
//    /// Creates a deep copy of this PokemonSet for MCTS simulation purposes.
//    /// </summary>
//    /// <returns>A new PokemonSet instance with copied Pokemon</returns>
//    public PokemonSet Copy()
//    {
//        return new PokemonSet
//        {
//            Pokemons = Pokemons.Select(pokemon => pokemon.Copy()).ToArray(),
//            Side = Side,
//        };
//    }
//}