using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceModifyTarget event.
/// Signature: Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove>
/// </summary>
public sealed record OnSourceModifyTargetEventInfo : EventHandlerInfo
{
    public OnSourceModifyTargetEventInfo(
        Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove> handler,
    int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.ModifyTarget;
        Prefix = EventPrefix.Source;
 Handler = handler;
 Priority = priority;
UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(void);
    }
}