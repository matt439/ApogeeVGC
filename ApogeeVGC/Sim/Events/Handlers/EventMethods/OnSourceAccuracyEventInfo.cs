using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceAccuracy event.
/// Modifies accuracy when this Pokemon is the source.
/// Signature: (Battle battle, int accuracy, Pokemon target, Pokemon source, ActiveMove move) => IntBoolVoidUnion?
/// </summary>
public sealed record OnSourceAccuracyEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSourceAccuracy event handler.
  /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSourceAccuracyEventInfo(
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?> handler,
int? priority = null,
     bool usesSpeed = true)
{
        Id = EventId.Accuracy;
        Prefix = EventPrefix.Source;
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
 ExpectedReturnType = typeof(IntBoolVoidUnion);
    }
}
