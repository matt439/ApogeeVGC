using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceAfterMoveSecondarySelf event.
/// Signature: Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>
/// </summary>
public sealed record OnSourceAfterMoveSecondarySelfEventInfo : EventHandlerInfo
{
    public OnSourceAfterMoveSecondarySelfEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion> handler,
    int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.AfterMoveSecondarySelf;
        Prefix = EventPrefix.Source;
 Handler = handler;
 Priority = priority;
UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(BoolVoidUnion);
    }
}