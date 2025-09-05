using ApogeeVGC.Sim.GameObjects;

namespace ApogeeVGC.Sim.PokemonClasses;

public class Team
{
    public required Trainer Trainer { get; init; }
    public required PokemonSet PokemonSet { get; init; }
    public bool PrintDebug { get; init; }
    public int? Slot1PokemonIndex
    {
        get;
        set
        {
            if (value == Slot2PokemonIndex)
            {
                throw new ArgumentException("Slot1 and Slot2 Pokemon indexes must be different.");
            }
            if (value < 0 || value >= PokemonSet.PokemonCount)
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                    "ActivePokemon must be within the range of the Pokemon set.");
            }
            field = value;
        }
    } = 0;

    public int? Slot2PokemonIndex
    {
        get;
        set
        {
            if (value == Slot1PokemonIndex)
            {
                throw new ArgumentException("Slot1 and Slot2 Pokemon indexes must be different.");
            }

            if (value < 0 || value >= PokemonSet.PokemonCount)
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                    "ActivePokemon must be within the range of the Pokemon set.");
            }

            field = value;
        }
    } = 1;
    public Pokemon? Slot1Pokemon
    {
        get
        {
            if (PokemonSet.PokemonCount == 0)
            {
                throw new InvalidOperationException("No Pokemon in the team.");
            }
            if (Slot1PokemonIndex is null)
            {
                return null;
            }
            return PokemonSet.Pokemons[(int)Slot1PokemonIndex] ??
                   throw new InvalidOperationException("Active Pokemon is null.");
        }
        
    }
    public Pokemon? Slot2Pokemon
    {
        get
        {
            if (PokemonSet.PokemonCount < 2)
            {
                throw new InvalidOperationException("No second Pokemon in the team.");
            }
            if (Slot2PokemonIndex is null)
            {
                return null;
            }
            return PokemonSet.Pokemons[(int)Slot2PokemonIndex] ??
                   throw new InvalidOperationException("Active Pokemon is null.");
        }

    }

    //public int ActivePokemonIndex
    //{
    //    get;
    //    set
    //    {
    //        if (value < 0 || value >= PokemonSet.PokemonCount)
    //        {
    //            throw new ArgumentOutOfRangeException(nameof(value),
    //                "ActivePokemon must be within the range of the Pokemon set.");
    //        }

    //        if (PokemonSet.Pokemons[value] == null)
    //        {
    //            throw new InvalidOperationException("Active Pokemon cannot be null.");
    //        }

    //        field = value;
    //    }
    //} = 0;
    //public Pokemon ActivePokemon
    //{
    //    get
    //    {
    //        if (PokemonSet.PokemonCount == 0)
    //        {
    //            throw new InvalidOperationException("No Pokemon in the team.");
    //        }
    //        return PokemonSet.Pokemons[ActivePokemonIndex] ??
    //               throw new InvalidOperationException("Active Pokemon is null.");
    //    }
    //}
    public Pokemon?[] AllActivePokemon => [Slot1Pokemon, Slot2Pokemon];
    public int AllActivePokemonCount => AllActivePokemon.Length;
    public int[] SwitchOptionIndexes => Enumerable.Range(0, PokemonSet.PokemonCount)
            .Where(i => i != Slot1PokemonIndex && i != Slot2PokemonIndex &&
                        !PokemonSet.Pokemons[i].IsFainted)
            .ToArray();
    public int SwitchOptionsCount => SwitchOptionIndexes.Length;
    public bool IsDefeated => PokemonSet.AllFainted;
    public int HealthTeamTotal => PokemonSet.Pokemons.Sum(p => p.CurrentHp);

    public Pokemon? GetPokemon(SlotId slot)
    {
        return slot switch
        {
            SlotId.Slot1 => Slot1Pokemon,
            SlotId.Slot2 => Slot2Pokemon,
            _ => throw new ArgumentOutOfRangeException(nameof(slot), slot, null),
        };
    }

    public Pokemon? GetAlly(SlotId slot)
    {
        return slot switch
        {
            SlotId.Slot1 => Slot2Pokemon,
            SlotId.Slot2 => Slot1Pokemon,
            _ => throw new ArgumentOutOfRangeException(nameof(slot), slot, null),
        };
    }

    public void SetPokemonIndex(int? index, SlotId slot)
    {
        switch (slot)
        {
            case SlotId.Slot1:
                Slot1PokemonIndex = index;
                break;
            case SlotId.Slot2:
                Slot2PokemonIndex = index;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(slot), slot, null);
        }
    }

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
            //ActivePokemonIndex = ActivePokemonIndex,
            Slot1PokemonIndex = Slot1PokemonIndex,
            Slot2PokemonIndex = Slot2PokemonIndex,
            PrintDebug = PrintDebug,
        };
    }

    //public override string ToString()
    //{
    //    string line1 = $"Team: {Trainer.Name}, Active:\n";
    //    string line2 = $"{ActivePokemon}\n";
    //    string line3 = $"Pokemon Count: {PokemonSet.PokemonCount}, " + 
    //                   "IsFainted: {PokemonSet.FaintedCount}/{PokemonSet.PokemonCount}";
    //    return line1 + line2 + line3 ;
    //}
    //public void Print()
    //{
    //    Console.WriteLine(ToString());
    //}
}