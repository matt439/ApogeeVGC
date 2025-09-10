using ApogeeVGC.Player;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.GameObjects;

namespace ApogeeVGC.Sim.PokemonClasses;

public class Side
{
    public required Team Team { get; init; }
    public required PlayerId PlayerId { get; init; }
    public required SideId SideId { get; init; }
    public required BattleFormat BattleFormat { get; init; }
    public bool PrintDebug { get; init; }

    public required Pokemon Slot1
    {
        get;
        set
        {
            value.SlotId = SlotId.Slot1;
            field = value;
        }
    }

    public required Pokemon Slot2
    {
        get;
        set
        {
            value.SlotId = SlotId.Slot2;
            field = value;
        }
    }

    public required Pokemon Slot3
    {
        get;
        set
        {
            value.SlotId = SlotId.Slot3;
            field = value;
        }
    }

    public required Pokemon Slot4
    {
        get;
        set
        {
            value.SlotId = SlotId.Slot4;
            field = value;
        }
    }
    public required Pokemon Slot5
    {
        get;
        set
        {
            if (BattleFormat == BattleFormat.Doubles)
            {
                throw new InvalidOperationException("Cannot set Slot5 in Doubles format.");
            }
            value.SlotId = SlotId.Slot5;
            field = value;
        }
    }
    public required Pokemon Slot6
    {
        get;
        set
        {
            if (BattleFormat == BattleFormat.Doubles)
            {
                throw new InvalidOperationException("Cannot set Slot6 in Doubles format.");
            }
            value.SlotId = SlotId.Slot6;
            field = value;
        }
    }

    public IEnumerable<Pokemon> AllSlots
    {
        get
        {
            yield return Slot1;
            yield return Slot2;
            yield return Slot3;
            yield return Slot4;
            if (BattleFormat == BattleFormat.Singles)
            {
                yield return Slot5;
                yield return Slot6;
            }
        }
    }

    //public IEnumerable<(SlotId, Pokemon)> AllSlotsWithIds
    //{
    //    get
    //    {
    //        yield return (SlotId.Slot1, Slot1);
    //        yield return (SlotId.Slot2, Slot2);
    //        yield return (SlotId.Slot3, Slot3);
    //        yield return (SlotId.Slot4, Slot4);
    //        if (BattleFormat == BattleFormat.Singles)
    //        {
    //            yield return (SlotId.Slot5, Slot5);
    //            yield return (SlotId.Slot6, Slot6);
    //        }
    //    }
    //}

    public IEnumerable<Pokemon> SwitchOptionSlots
    {
        get
        {
            return BattleFormat switch
            {
                BattleFormat.Singles => new[] { Slot2, Slot3, Slot4, Slot5, Slot6 }.Where(p => !p.IsFainted),
                BattleFormat.Doubles => new[] { Slot3, Slot4 }.Where(p => !p.IsFainted),
                _ => throw new InvalidOperationException("Invalid battle format."),
            };
        }
    }

    public int SwitchOptionsCount => SwitchOptionSlots.Count();

    public bool IsDefeated
    {
        get
        {
            var allSlots = AllSlots;
            return allSlots.All(p => p.IsFainted);
        }
    }

    public int HealthTeamTotal
    {
        get
        {
            var allSlots = AllSlots;
            return allSlots.Sum(p => p.CurrentHp);
        }
    }

    public bool AnyTeraUsed
    {
        get
        {
            var allSlots = AllSlots;
            return allSlots.Any(p => p.IsTeraUsed);
        }
    }

    public int AlivePokemonCount => AllSlots.Count(p => !p.IsFainted);

    public Pokemon GetSlot(SlotId slotId) => slotId switch
    {
        SlotId.Slot1 => Slot1,
        SlotId.Slot2 => Slot2,
        SlotId.Slot3 => Slot3,
        SlotId.Slot4 => Slot4,
        SlotId.Slot5 => Slot5,
        SlotId.Slot6 => Slot6,
        _ => null,
    } ?? throw new InvalidOperationException($"Slot {slotId} is null." );

    public void SetSlot(SlotId slotId, Pokemon pokemon)
    {
        switch (slotId)
        {
            case SlotId.Slot1: Slot1 = pokemon; break;
            case SlotId.Slot2: Slot2 = pokemon; break;
            case SlotId.Slot3: Slot3 = pokemon; break;
            case SlotId.Slot4: Slot4 = pokemon; break;
            case SlotId.Slot5: Slot5 = pokemon; break;
            case SlotId.Slot6: Slot6 = pokemon; break;
            default:
                throw new ArgumentOutOfRangeException(nameof(slotId), slotId, null);
        }
    }

    public void SetSlotsWithCopies(IReadOnlyList<Pokemon> pokemons)
    {
        switch (BattleFormat)
        {
            case BattleFormat.Singles:
                if (pokemons.Count != 6)
                {
                    throw new ArgumentException("Must provide exactly 6 Pokémon for Singles format.");
                }
                Slot1 = pokemons[0].Copy();
                Slot2 = pokemons[1].Copy();
                Slot3 = pokemons[2].Copy();
                Slot4 = pokemons[3].Copy();
                Slot5 = pokemons[4].Copy();
                Slot6 = pokemons[5].Copy();
                break;
            case BattleFormat.Doubles:

                if (pokemons.Count != 4)
                {
                    throw new ArgumentException("Must provide exactly 4 Pokémon for Doubles format.");
                }
                Slot1 = pokemons[0].Copy();
                Slot2 = pokemons[1].Copy();
                Slot3 = pokemons[2].Copy();
                Slot4 = pokemons[3].Copy();
                break;
            default:
                throw new InvalidOperationException("Invalid battle format.");
        }
    }

    public void SwitchSlots(SlotId activeSlot, SlotId benchSlot)
    {
        if (!IsValidActiveSlot(activeSlot))
        {
            throw new ArgumentException($"Slot {activeSlot} is not a valid active slot for {BattleFormat} format.");
        }
        if (!IsValidBenchSlot(benchSlot))
        {
            throw new ArgumentException($"Slot {benchSlot} is not a valid bench slot for {BattleFormat} format.");
        }
        Pokemon activePokemon = GetSlot(activeSlot);
        Pokemon benchPokemon = GetSlot(benchSlot);
        if (benchPokemon.IsFainted)
        {
            throw new InvalidOperationException($"Cannot switch in fainted Pokemon in slot {benchSlot}.");
        }
        // Perform the switch
        SetSlot(activeSlot, benchPokemon);
        SetSlot(benchSlot, activePokemon);
        // Update their SlotId properties
        activePokemon.SlotId = benchSlot;
        benchPokemon.SlotId = activeSlot;
    }

    private bool IsValidActiveSlot(SlotId slot)
    {
        return BattleFormat switch
        {
            BattleFormat.Singles => slot is SlotId.Slot1,
            BattleFormat.Doubles => slot is SlotId.Slot1 or SlotId.Slot2,
            _ => throw new InvalidOperationException("Invalid battle format."),
        };
    }

    private bool IsValidBenchSlot(SlotId slot)
    {
        return BattleFormat switch
        {
            BattleFormat.Singles => slot is SlotId.Slot2 or SlotId.Slot3 or SlotId.Slot4 or
                SlotId.Slot5 or SlotId.Slot6,
            BattleFormat.Doubles => slot is SlotId.Slot3 or SlotId.Slot4,
            _ => throw new InvalidOperationException("Invalid battle format."),
        };
    }

    /// <summary>
    /// Creates a deep copy of this Side for MCTS simulation purposes.
    /// </summary>
    /// <returns>A new Side instance with copied state</returns>
    public Side Copy()
    {
        return new Side
        {
            PlayerId = PlayerId, // Value type, safe to copy
            Team = Team.Copy(),
            PrintDebug = PrintDebug, // Added missing PrintDebug
            SideId = SideId, // Value type, safe to copy
            Slot1 = Slot1.Copy(),
            Slot2 = Slot2.Copy(),
            Slot3 = Slot3.Copy(),
            Slot4 = Slot4.Copy(),
            Slot5 = Slot5.Copy(),
            Slot6 = Slot6.Copy(),
            BattleFormat = BattleFormat,
        };
    }
}