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

    public required Pokemon Slot1 { get; set; }
    public required Pokemon Slot2 { get; set; }
    public required Pokemon Slot3 { get; set; }
    public required Pokemon Slot4 { get; set; }
    public required Pokemon Slot5
    {
        get;
        set
        {
            if (BattleFormat == BattleFormat.Doubles)
            {
                throw new InvalidOperationException("Cannot set Slot5 in Doubles format.");
            }
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

    public IEnumerable<Pokemon> SwitchOptionSlots => AllSlots
        .Where(p => p is { IsFainted: false });

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