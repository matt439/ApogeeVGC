using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Sim.Utils.Extensions;

public static class SlotIdTools
{
    public static SlotId GetAllySlot(this SlotId slotId)
    {
        return slotId switch
        {
            SlotId.Slot1 => SlotId.Slot2,
            SlotId.Slot2 => SlotId.Slot1,
            SlotId.Slot3 or SlotId.Slot4 => throw new InvalidOperationException("Bench slots do not have ally slots."),
            _ => throw new ArgumentOutOfRangeException(nameof(slotId), slotId, null)
        };
    }
}