using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeImmunity event.
/// Signature: Action<Battle, PokemonType, Pokemon>
/// </summary>
public sealed record OnFoeImmunityEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnFoeImmunityEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Immunity;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeImmunityEventInfo Create(
        Action<Battle, PokemonType, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeImmunityEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.SourceType!.Value,
                context.GetSourceOrTargetPokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
