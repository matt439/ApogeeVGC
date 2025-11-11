using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifySecondaries event.
/// Modifies secondary effects of a move.
/// Signature: (Battle battle, List<SecondaryEffect> secondaries, Pokemon target, Pokemon source, ActiveMove move) => void
/// </summary>
public sealed record OnModifySecondariesEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnModifySecondaries event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnModifySecondariesEventInfo(
        Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
 bool usesSpeed = true)
    {
      Id = EventId.ModifySecondaries;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
      [
            typeof(Battle),
     typeof(List<SecondaryEffect>),
    typeof(Pokemon),
   typeof(Pokemon),
       typeof(ActiveMove),
        ];
    ExpectedReturnType = typeof(void);
    }
}
