using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceOverrideAction event.
/// Signature: Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>
/// </summary>
public sealed record OnSourceOverrideActionEventInfo : EventHandlerInfo
{
    public OnSourceOverrideActionEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion> handler,
    int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.OverrideAction;
        Prefix = EventPrefix.Source;
 Handler = handler;
 Priority = priority;
UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(DelegateVoidUnion);
    }
}