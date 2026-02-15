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
    /// Creates a new OnFieldResidual event handler using the legacy strongly-typed pattern.
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

    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;

    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, Field, TargetPokemon, SourceEffect
    /// </summary>
    public OnFieldResidualEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        int? order = null,
        int? subOrder = null,
        bool usesSpeed = true)
    {
        Id = EventId.FieldResidual;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        Order = order;
        SubOrder = subOrder;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFieldResidualEventInfo Create(
        Action<Battle, Field, Pokemon, IEffect> handler,
        int? priority = null,
        int? order = null,
        int? subOrder = null,
        bool usesSpeed = true)
    {
        return new OnFieldResidualEventInfo(
            context =>
            {
                handler(
                    context.Battle,
                    context.Battle.Field,
                    context.GetTargetPokemon(),
                    context.GetSourceEffect<IEffect>()
                );
                return null;
            },
            priority,
            order,
            subOrder,
            usesSpeed
        );
    }
}
