using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.PokemonClasses;

public enum MoveSlotId
{
    Move1 = 0,
    Move2 = 1,
    Move3 = 2,
    Move4 = 3,
}

public class MoveSlot
{
    public MoveId Id { get; init; }
    public MoveId Move { get; init; }
    public int Pp { get; set; }
    public int MaxPp { get; set; }
    public MoveTarget? Target { get; set; }
    public BoolHiddenUnion Disabled { get; set; } = false;
    public ConditionId? DisabledSource { get; set; }
    public bool Used { get; set; }
    public bool? Virtual { get; set; }
}