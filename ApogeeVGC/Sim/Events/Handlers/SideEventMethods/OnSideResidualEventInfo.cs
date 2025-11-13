using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Events.Handlers.SideEventMethods;

/// <summary>
/// Event handler info for OnSideResidual event (side-specific).
/// Triggered for residual side condition effects (each turn).
/// Signature: Action&lt;Battle, Side, Pokemon, IEffect&gt;
/// </summary>
public sealed record OnSideResidualEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSideResidual event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="order">Execution order value</param>
    /// <param name="subOrder">Execution sub-order value</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSideResidualEventInfo(
      Action<Battle, Side, Pokemon, IEffect> handler,
        int? priority = null,
        int? order = null,
  int? subOrder = null,
     bool usesSpeed = true)
    {
        Id = EventId.SideResidual;
        Prefix = EventPrefix.None;
   Handler = handler;
        Priority = priority;
        Order = order;
        SubOrder = subOrder;
        UsesSpeed = usesSpeed;
   ExpectedParameterTypes = [typeof(Battle), typeof(Side), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
