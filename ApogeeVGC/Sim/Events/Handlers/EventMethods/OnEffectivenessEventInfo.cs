using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnEffectiveness event.
/// Modifies type effectiveness calculation.
/// Signature: (Battle battle, int typeMod, Pokemon? target, PokemonType type, ActiveMove move) => IntVoidUnion
/// </summary>
public sealed record OnEffectivenessEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnEffectiveness event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
  /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnEffectivenessEventInfo(
  Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion> handler,
int? priority = null,
        bool usesSpeed = true)
    {
    Id = EventId.Effectiveness;
Handler = handler;
Priority = priority;
      UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
  [
  typeof(Battle),
       typeof(int),
     typeof(Pokemon),
     typeof(PokemonType),
  typeof(ActiveMove),
    ];
  ExpectedReturnType = typeof(IntVoidUnion);
    }
}
