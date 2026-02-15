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
    /// <summary>
    /// Creates a new OnEatItem event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnEatItemEventInfo(
        Action<Battle, Item, Pokemon, Pokemon?, IEffect?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.EatItem;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
            typeof(Battle),
            typeof(Item),
            typeof(Pokemon),
            typeof(Pokemon),
            typeof(IEffect),
        ];
        ExpectedReturnType = typeof(void);

        // Nullability: source and effect are optional (nullable)
        ParameterNullability = [false, false, false, true, true];
        ReturnTypeNullable = false;

        // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
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
                (Item)context.Effect!,
                context.GetTargetPokemon(),
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