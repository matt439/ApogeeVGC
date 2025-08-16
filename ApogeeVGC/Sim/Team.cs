using ApogeeVGC.Data;
using ApogeeVGC.Player;

namespace ApogeeVGC.Sim;

public class Team
{
    public required Trainer Trainer { get; init; }
    public required PokemonSet PokemonSet { get; init; }

    public required int ActivePokemonIndex
    {
        get;
        set
        {
            if (value < 0 || value >= PokemonSet.PokemonCount)
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                    "ActivePokemon must be within the range of the Pokemon set.");
            }
            if (PokemonSet.Pokemons[value] == null)
            {
                throw new InvalidOperationException("Active Pokemon cannot be null.");
            }
            field = value;
        }
    }
    public Pokemon ActivePokemon
    {
        get
        {
            if (PokemonSet.PokemonCount == 0)
            {
                throw new InvalidOperationException("No Pokemon in the team.");
            }
            return PokemonSet.Pokemons[ActivePokemonIndex] ??
                   throw new InvalidOperationException("Active Pokemon is null.");
        }
    }
    public int[] SwitchOptionIndexes => Enumerable.Range(0, PokemonSet.PokemonCount)
            .Where(i => i != ActivePokemonIndex && !PokemonSet.Pokemons[i].Fainted)
            .ToArray();
    public bool IsDefeated => PokemonSet.AllFainted;
    public override string ToString()
    {
        string line1 = $"Team: {Trainer.Name}, Active:\n";
        string line2 = $"{ActivePokemon}\n";
        string line3 = $"Pokemon Count: {PokemonSet.PokemonCount}, " + 
                       "Fainted: {PokemonSet.FaintedCount}/{PokemonSet.PokemonCount}";
        return line1 + line2 + line3 ;
    }
    public void Print()
    {
        Console.WriteLine(ToString());
    }
}

public static class TeamGenerator
{
    public static Team GenerateTestTeam(Library library)
    {
        return new Team
        {
            Trainer = TrainerGenerator.GenerateTestTrainer(),
            PokemonSet = PokemonBuilder.BuildTestSet(library),
            ActivePokemonIndex = 0
        };
    }
}