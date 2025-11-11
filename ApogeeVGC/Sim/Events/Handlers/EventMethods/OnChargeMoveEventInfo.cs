using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnChargeMove event.
/// Triggered when a Pokemon is charging a move (e.g., Solar Beam).
/// Signature: (Battle battle, Pokemon source, Pokemon target, ActiveMove move) => BoolVoidUnion
/// </summary>
public sealed record OnChargeMoveEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnChargeMove event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnChargeMoveEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion> handler,
        int? priority = null,
  bool usesSpeed = true)
    {
   Id = EventId.ChargeMove;
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
     ExpectedReturnType = typeof(BoolVoidUnion);
  }
}
