using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceModifySecondaries event.
/// Signature: Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>
/// </summary>
public sealed record OnSourceModifySecondariesEventInfo : EventHandlerInfo
{
    public OnSourceModifySecondariesEventInfo(
        Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove> handler,
    int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.ModifySecondaries;
        Prefix = EventPrefix.Source;
 Handler = handler;
 Priority = priority;
UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(List<SecondaryEffect>), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(void);
    }
}