using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnResidual event.
/// Triggered at the end of each turn for residual effects.
/// Signature: (Battle battle, Pokemon target, Pokemon source, IEffect effect) => void
/// </summary>
public sealed record OnResidualEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnResidual event handler.
/// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="order">Execution order (lower executes first)</param>
    /// <param name="subOrder">Sub-order for fine-grained ordering</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnResidualEventInfo(
        Action<Battle, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
        int? order = null,
        int? subOrder = null,
        bool usesSpeed = true)
    {
        Id = EventId.Residual;
        Handler = handler;
        Priority = priority;
        Order = order;
        SubOrder = subOrder;
    UsesSpeed = usesSpeed;
 ExpectedParameterTypes =
 [
     typeof(Battle), 
          typeof(Pokemon), 
       typeof(Pokemon), 
 typeof(IEffect),
 ];
      ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
