using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeAfterEachBoost event.
/// Triggered after each individual stat boost is applied to a foe.
/// Signature: (Battle battle, SparseBoostsTable boost, Pokemon target, Pokemon source) => void
/// </summary>
public sealed record OnFoeAfterEachBoostEventInfo : EventHandlerInfo
{
  /// <summary>
    /// Creates a new OnFoeAfterEachBoost event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFoeAfterEachBoostEventInfo(
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.AfterEachBoost;
        Prefix = EventPrefix.Foe;
Handler = handler;
  Priority = priority;
 UsesSpeed = usesSpeed;
 ExpectedParameterTypes =
   [
  typeof(Battle),
            typeof(SparseBoostsTable),
  typeof(Pokemon),
     typeof(Pokemon),
        ];
  ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
