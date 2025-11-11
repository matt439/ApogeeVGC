using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifyAtk event.
/// Modifies the Attack stat.
/// Signature: (Battle battle, int relayVar, Pokemon source, Pokemon target, ActiveMove move) => DoubleVoidUnion
/// </summary>
public sealed record OnModifyAtkEventInfo : EventHandlerInfo
{
 /// <summary>
    /// Creates a new OnModifyAtk event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnModifyAtkEventInfo(
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
        Id = EventId.ModifyAtk;
        Handler = handler;
  Priority = priority;
   UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
      [
      typeof(Battle),
      typeof(int),
 typeof(Pokemon),
        typeof(Pokemon),
 typeof(ActiveMove),
        ];
 ExpectedReturnType = typeof(DoubleVoidUnion);
    }
}
