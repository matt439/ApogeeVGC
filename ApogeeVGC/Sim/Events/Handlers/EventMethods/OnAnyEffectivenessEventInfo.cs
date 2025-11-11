using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyEffectiveness event.
/// Signature: Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion>
/// </summary>
public sealed record OnAnyEffectivenessEventInfo : EventHandlerInfo
{
    public OnAnyEffectivenessEventInfo(
        Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.Effectiveness;
Prefix = EventPrefix.Any;
        Handler = handler;
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(PokemonType), typeof(ActiveMove)];
        ExpectedReturnType = typeof(IntVoidUnion);
    }
}