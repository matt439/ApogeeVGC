using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAccuracy event.
/// Modifies the accuracy of a move.
/// Signature: (Battle battle, int accuracy, Pokemon target, Pokemon source, ActiveMove move) => IntBoolVoidUnion?
/// </summary>
public sealed record OnAccuracyEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnAccuracy event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnAccuracyEventInfo(
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
 {
        Id = EventId.Accuracy;
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
