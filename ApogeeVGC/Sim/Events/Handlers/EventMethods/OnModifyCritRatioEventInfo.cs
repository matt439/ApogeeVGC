using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifyCritRatio event.
/// Modifies the critical hit ratio.
/// Signature: (Battle battle, int relayVar, Pokemon source, Pokemon target, ActiveMove move) => DoubleVoidUnion
/// </summary>
public sealed record OnModifyCritRatioEventInfo : EventHandlerInfo
{
  /// <summary>
    /// Creates a new OnModifyCritRatio event handler.
/// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnModifyCritRatioEventInfo(
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
  Id = EventId.ModifyCritRatio;
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
