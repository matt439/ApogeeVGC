using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.BattleClasses;

public record FaintQueue
{
    public required Pokemon Target { get; init; }
    public Pokemon? Source { get; init; }
    public IEffect? Effect { get; init; }
}