using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyNegateImmunity event.
/// Signature: Func<Battle, Pokemon, PokemonType?, BoolVoidUnion>
/// </summary>
public sealed record OnAnyNegateImmunityEventInfo : EventHandlerInfo
{
    public OnAnyNegateImmunityEventInfo(
        Func<Battle, Pokemon, PokemonType?, BoolVoidUnion> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.NegateImmunity;
Prefix = EventPrefix.Any;
        Handler = handler;
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(PokemonType)];
        ExpectedReturnType = typeof(BoolVoidUnion);
    }
}