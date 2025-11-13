using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.FieldEventMethods;

/// <summary>
/// Event handler info for OnFieldRestart event (field-specific).
/// Triggered when a field condition restarts/reactivates.
/// Signature: Action&lt;Battle, Field, Pokemon, IEffect&gt;
/// </summary>
public sealed record OnFieldRestartEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnFieldRestart event handler.
  /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFieldRestartEventInfo(
        Action<Battle, Field, Pokemon, IEffect> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
 Id = EventId.FieldRestart;
        Prefix = EventPrefix.None;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Field), typeof(Pokemon), typeof(IEffect)];
   ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
