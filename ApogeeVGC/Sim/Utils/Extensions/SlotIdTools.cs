using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Sim.Utils.Extensions;

public static class SlotIdTools
{
    public static PokemonSlotId GetAllySlot(this PokemonSlotId pokemonSlotId)
    {
        return pokemonSlotId switch
        {
            PokemonSlotId.Slot1 => PokemonSlotId.Slot2,
            PokemonSlotId.Slot2 => PokemonSlotId.Slot1,
            PokemonSlotId.Slot3 or PokemonSlotId.Slot4 => throw new InvalidOperationException("Bench slots do not have ally slots."),
            _ => throw new ArgumentOutOfRangeException(nameof(pokemonSlotId), pokemonSlotId, null)
        };
    }
}