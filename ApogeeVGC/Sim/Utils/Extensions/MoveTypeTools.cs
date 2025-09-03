using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Extensions;

public static class MoveTypeTools
{
    public static PokemonType ConvertToPokemonType(this MoveType type)
    {
        return type switch
        {
            MoveType.Normal => PokemonType.Normal,
            MoveType.Fire => PokemonType.Fire,
            MoveType.Water => PokemonType.Water,
            MoveType.Electric => PokemonType.Electric,
            MoveType.Grass => PokemonType.Grass,
            MoveType.Ice => PokemonType.Ice,
            MoveType.Fighting => PokemonType.Fighting,
            MoveType.Poison => PokemonType.Poison,
            MoveType.Ground => PokemonType.Ground,
            MoveType.Flying => PokemonType.Flying,
            MoveType.Psychic => PokemonType.Psychic,
            MoveType.Bug => PokemonType.Bug,
            MoveType.Rock => PokemonType.Rock,
            MoveType.Ghost => PokemonType.Ghost,
            MoveType.Dragon => PokemonType.Dragon,
            MoveType.Dark => PokemonType.Dark,
            MoveType.Steel => PokemonType.Steel,
            MoveType.Fairy => PokemonType.Fairy,
            MoveType.Stellar => throw new ArgumentOutOfRangeException(nameof(type),
                "Stellar type cannot be converted to Pokemon type."),
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Invalid move type.")
        };
    }

    public static string ConvertToString(this MoveType type)
    {
        return type switch
        {
            MoveType.Normal => "Normal",
            MoveType.Fire => "Fire",
            MoveType.Water => "Water",
            MoveType.Electric => "Electric",
            MoveType.Grass => "Grass",
            MoveType.Ice => "Ice",
            MoveType.Fighting => "Fighting",
            MoveType.Poison => "Poison",
            MoveType.Ground => "Ground",
            MoveType.Flying => "Flying",
            MoveType.Psychic => "Psychic",
            MoveType.Bug => "Bug",
            MoveType.Rock => "Rock",
            MoveType.Ghost => "Ghost",
            MoveType.Dragon => "Dragon",
            MoveType.Dark => "Dark",
            MoveType.Steel => "Steel",
            MoveType.Fairy => "Fairy",
            MoveType.Stellar => "Stellar",
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Invalid move type.")
        };
    }
}