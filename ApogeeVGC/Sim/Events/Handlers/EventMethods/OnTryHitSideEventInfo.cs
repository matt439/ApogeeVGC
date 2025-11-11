using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnTryHitSide event.
/// Triggered when attempting to hit a side with a move.
/// Signature: (Battle battle, Pokemon target, Pokemon source, ActiveMove move) => BoolEmptyVoidUnion?
/// </summary>
public sealed record OnTryHitSideEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnTryHitSide event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnTryHitSideEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?> handler,
   int? priority = null,
bool usesSpeed = true)
    {
   Id = EventId.TryHitSide;
        Handler = handler;
 Priority = priority;
   UsesSpeed = usesSpeed;
  ExpectedParameterTypes =
  [
typeof(Battle),
typeof(Pokemon),
     typeof(Pokemon),
typeof(ActiveMove),
  ];
    ExpectedReturnType = typeof(BoolEmptyVoidUnion);
    }
}
