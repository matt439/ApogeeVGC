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
                    context.GetTargetOrSourcePokemon(),
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
