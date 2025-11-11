using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyTryPrimaryHit event.
/// Signature: Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>
/// </summary>
public sealed record OnAnyTryPrimaryHitEventInfo : EventHandlerInfo
{
    public OnAnyTryPrimaryHitEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.TryPrimaryHit;
Prefix = EventPrefix.Any;
        Handler = handler;
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(IntBoolVoidUnion);
    }
}