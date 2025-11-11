using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifyDamagePhase2 event.
/// Modifies damage in phase 2 of damage calculation.
/// Signature: (Battle battle, int relayVar, Pokemon source, Pokemon target, ActiveMove move) => DoubleVoidUnion
/// </summary>
public sealed record OnModifyDamagePhase2EventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnModifyDamagePhase2 event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
/// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnModifyDamagePhase2EventInfo(
  Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
        int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.ModifyDamagePhase2;
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
