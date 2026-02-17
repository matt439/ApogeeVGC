using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Conditions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSideConditionStart event.
/// Triggered when a side condition starts.
/// Signature: (Battle battle, Side target, Pokemon source, Condition sideCondition) => void
/// </summary>
public sealed record OnSideConditionStartEventInfo : EventHandlerInfo
{
    public OnSideConditionStartEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SideConditionStart;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSideConditionStartEventInfo Create(
        Action<Battle, Side, Pokemon, Condition> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSideConditionStartEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetTargetSide(),
                context.GetTargetOrSourcePokemon(),
                context.GetSourceEffect<Condition>()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
