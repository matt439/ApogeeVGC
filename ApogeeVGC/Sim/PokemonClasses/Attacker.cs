using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.PokemonClasses;

public record Attacker
{
    public required Pokemon Source { get; init; }
    public int Damage { get; init; }
    public bool ThisTurn { get; init; }
    public MoveId? Move { get; init; }
    public PokemonSlot PokemonSlot { get; init; } = new(SideId.P1, 0);
    public IntFalseUnion? DamageValue { get; init; }
}