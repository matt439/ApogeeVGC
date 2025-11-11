using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceHit event.
/// Signature: Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>
/// </summary>
public sealed record OnSourceHitEventInfo : EventHandlerInfo
{
    public OnSourceHitEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?> handler,
    int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.Hit;
        Prefix = EventPrefix.Source;
 Handler = handler;
 Priority = priority;
UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(BoolEmptyVoidUnion);
    }
}