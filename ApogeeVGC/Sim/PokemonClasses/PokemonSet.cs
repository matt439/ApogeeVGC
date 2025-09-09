﻿using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Sim.PokemonClasses;

/// <summary>
/// The set of Pokémon a player has in a battle.
/// The Pokemon in the set are not modified during battle.
/// Instead, Pokemon which will battle are copied into the Side's slots.
/// In a doubles battle, the 2 unused Pokemon can be accessed from the set.
/// </summary>
public record PokemonSet
{
    public required IReadOnlyList<Pokemon> Pokemons
    {
        get;
        init
        {
            if (value == null || value.Count == 0)
            {
                throw new ArgumentException("Pokemon set must contain at least one Pokemon.");
            }
            if (value.Count > 6)
            {
                throw new ArgumentException("Pokemon set cannot contain more than 6 Pokemon.");
            }
            field = value;
        }
    }
    public int PokemonCount => Pokemons.Count;
    //public Pokemon[] AlivePokemon => Pokemons.Where(pokemon => !pokemon.IsFainted).ToArray();
    //public int AlivePokemonCount => AlivePokemon.Length;
    //public int FaintedCount => PokemonCount - AlivePokemonCount;
    //public bool AllFainted => AlivePokemonCount == 0;
    //public bool AnyTeraUsed => Pokemons.Any(pokemon => pokemon.IsTeraUsed);
    public required SideId SideId { get; init; }

    /// /// <summary>
    /// Creates a deep copy of this PokemonSet for MCTS simulation purposes.
    /// </summary>
    /// <returns>A new PokemonSet instance with copied Pokemon</returns>
    public PokemonSet Copy()
    {
        return new PokemonSet
        {
            Pokemons = Pokemons.Select(pokemon => pokemon.Copy()).ToArray(),
            SideId = SideId,
        };
    }
}