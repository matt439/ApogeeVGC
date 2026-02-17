using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnEatItem event.
/// Triggered when a Pokemon eats an item (like a berry).
/// TypeScript signature: onEatItem?: (this: Battle, item: Item, pokemon: Pokemon, source?: Pokemon, effect?: Effect) => void
/// C# Signature: (Battle battle, Item item, Pokemon pokemon, Pokemon? source, IEffect? effect) => void
/// </summary>
public sealed record OnEatItemEventInfo : EventHandlerInfo
{
    public OnEatItemEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.EatItem;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnEatItemEventInfo Create(
        Action<Battle, Item, Pokemon, Pokemon?, IEffect?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnEatItemEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetEffectParam<Item>(),
                context.GetTargetOrSourcePokemon(),
                context.SourcePokemon,
                context.TryGetSourceEffect<IEffect>()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
