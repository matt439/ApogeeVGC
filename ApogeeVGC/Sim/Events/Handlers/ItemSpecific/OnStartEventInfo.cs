using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.ItemSpecific;

/// <summary>
/// Event handler info for Item.OnStart event.
/// Signature: (Battle, Pokemon) => void
/// </summary>
public sealed record OnStartEventInfo : EventHandlerInfo
{
    public OnStartEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Start;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnStartEventInfo Create(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnStartEventInfo(
            context =>
            {
                handler(context.Battle, context.GetTargetOrSourcePokemon());
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
