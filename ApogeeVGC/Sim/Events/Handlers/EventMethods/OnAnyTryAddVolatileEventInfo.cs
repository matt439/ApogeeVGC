using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyTryAddVolatile event.
/// Signature: Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>
/// </summary>
public sealed record OnAnyTryAddVolatileEventInfo : EventHandlerInfo
{
    public OnAnyTryAddVolatileEventInfo(
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.TryAddVolatile;
Prefix = EventPrefix.Any;
        Handler = handler;
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(Condition), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(BoolVoidUnion);
    }
}