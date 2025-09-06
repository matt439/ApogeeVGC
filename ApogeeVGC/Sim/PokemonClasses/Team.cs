using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.GameObjects;

namespace ApogeeVGC.Sim.PokemonClasses;

public class Team
{
    public required Trainer Trainer { get; init; }
    public required PokemonSet PokemonSet { get; init; }
    public bool PrintDebug { get; init; }
    public int ActivePokemonIndex
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
    } = 0;
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
    // TODO: Need to update this with doubles
    public Pokemon[] AllActivePokemon => [ActivePokemon];
    public int AllActivePokemonCount => AllActivePokemon.Length;
    public int[] SwitchOptionIndexes => Enumerable.Range(0, PokemonSet.PokemonCount)
            .Where(i => i != ActivePokemonIndex && !PokemonSet.Pokemons[i].IsFainted)
            .ToArray();
    public int SwitchOptionsCount => SwitchOptionIndexes.Length;
    public bool IsDefeated => PokemonSet.AllFainted;
    public int HealthTeamTotal => PokemonSet.Pokemons.Sum(p => p.CurrentHp);
    public required SideId SideId { get; init; }

    /// <summary>
    /// Creates a deep copy of this Team for MCTS simulation purposes.
    /// </summary>
    /// <returns>A new Team instance with copied state</returns>
    public Team Copy()
    {
        return new Team
        {
            Trainer = Trainer, // Trainer is immutable, safe to share
            PokemonSet = PokemonSet.Copy(),
            ActivePokemonIndex = ActivePokemonIndex,
            PrintDebug = PrintDebug,
            SideId = SideId,
        };
    }

    public override string ToString()
    {
        string line1 = $"Team: {Trainer.Name}, Active:\n";
        string line2 = $"{ActivePokemon}\n";
        string line3 = $"Pokemon Count: {PokemonSet.PokemonCount}, " + 
                       "IsFainted: {PokemonSet.FaintedCount}/{PokemonSet.PokemonCount}";
        return line1 + line2 + line3 ;
    }
    public void Print()
    {
        Console.WriteLine(ToString());
    }
}