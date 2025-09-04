using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Methods;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Effects;

public record EffectMethods : ITestMethods
{
    public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnSourceAfterFaint { get; init; }
}