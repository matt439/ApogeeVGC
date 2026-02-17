using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeMaybeTrapPokemon event.
/// Triggered to potentially trap a foe Pokemon.
/// Signature: (Battle battle, Pokemon pokemon, Pokemon? source) => void
/// </summary>
public sealed record OnFoeMaybeTrapPokemonEventInfo : EventHandlerInfo
{
    public OnFoeMaybeTrapPokemonEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.MaybeTrapPokemon;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeMaybeTrapPokemonEventInfo Create(
        Action<Battle, Pokemon, Pokemon?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeMaybeTrapPokemonEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon(),
                context.SourcePokemon
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
