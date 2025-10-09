using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.PokemonClasses;

public record Attacker
{
    public required Pokemon Source { get; init; }
    public int Damage { get; init; }
    public bool ThisTurn { get; init; }
    public Move? Move { get; init; }
    public PokemonSlot PokemonSlot { get; init; } = new();
    public IntBoolUnion? DamageValue { get; init; }
}