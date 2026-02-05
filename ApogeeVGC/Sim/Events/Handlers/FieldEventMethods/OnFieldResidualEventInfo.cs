using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.FieldEventMethods;

/// <summary>
/// Event handler info for OnFieldResidual event (field-specific).
/// Triggered for residual field condition effects (each turn).
/// Signature: Action&lt;Battle, Field, Pokemon, IEffect&gt;
/// </summary>
public sealed record OnFieldResidualEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnFieldResidual event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="order">Execution order value</param>
    /// <param name="subOrder">Execution sub-order value</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFieldResidualEventInfo(
     Action<Battle, Field, Pokemon, IEffect> handler,
 int? priority = null,
        int? order = null,
        int? subOrder = null,
        bool usesSpeed = true)
    {
   Id = EventId.FieldResidual;
        Prefix = EventPrefix.None;
        Handler = handler;
  Priority = priority;
        Order = order;
        SubOrder = subOrder;
   UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Field), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: Pokemon parameter (index 2) can be null for field-level residual events like weather
        ParameterNullability = new[] { false, false, true, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
