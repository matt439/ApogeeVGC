using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Extensions;

public static class PokemonTypeTools
{
    public static MoveType ConvertToMoveType(this PokemonType type)
    {
        return type switch
        {
            PokemonType.Normal => MoveType.Normal,
            PokemonType.Fire => MoveType.Fire,
            PokemonType.Water => MoveType.Water,
            PokemonType.Electric => MoveType.Electric,
            PokemonType.Grass => MoveType.Grass,
            PokemonType.Ice => MoveType.Ice,
            PokemonType.Fighting => MoveType.Fighting,
            PokemonType.Poison => MoveType.Poison,
            PokemonType.Ground => MoveType.Ground,
            PokemonType.Flying => MoveType.Flying,
            PokemonType.Psychic => MoveType.Psychic,
            PokemonType.Bug => MoveType.Bug,
            PokemonType.Rock => MoveType.Rock,
            PokemonType.Ghost => MoveType.Ghost,
            PokemonType.Dragon => MoveType.Dragon,
            PokemonType.Dark => MoveType.Dark,
            PokemonType.Steel => MoveType.Steel,
            PokemonType.Fairy => MoveType.Fairy,
            PokemonType.Unknown => throw new ArgumentOutOfRangeException(nameof(type),
                "Unknown type cannot be converted to MoveType."),
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Invalid Pokemon type.")
        };
    }

    /// <summary>
    /// Converts a PokemonType to its protocol string representation.
    /// </summary>
    public static string ConvertToString(this PokemonType type)
    {
        return type switch
        {
            PokemonType.Normal => "Normal",
            PokemonType.Fire => "Fire",
            PokemonType.Water => "Water",
            PokemonType.Electric => "Electric",
            PokemonType.Grass => "Grass",
            PokemonType.Ice => "Ice",
            PokemonType.Fighting => "Fighting",
            PokemonType.Poison => "Poison",
            PokemonType.Ground => "Ground",
            PokemonType.Flying => "Flying",
            PokemonType.Psychic => "Psychic",
            PokemonType.Bug => "Bug",
            PokemonType.Rock => "Rock",
            PokemonType.Ghost => "Ghost",
            PokemonType.Dragon => "Dragon",
            PokemonType.Dark => "Dark",
            PokemonType.Steel => "Steel",
            PokemonType.Fairy => "Fairy",
            PokemonType.Unknown => "???",
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Invalid Pokemon type.")
        };
    }
}