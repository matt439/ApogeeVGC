using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyEatItem event (pokemon/ally-specific).
/// Triggered when ally eats item.
/// Signature: Action<Battle, Item, Pokemon>
/// </summary>
public sealed record OnAllyEatItemEventInfo : EventHandlerInfo
{
    public OnAllyEatItemEventInfo(
    Action<Battle, Item, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.EatItem;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Item), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
  }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAllyEatItemEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.EatItem;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAllyEatItemEventInfo Create(
        Action<Battle, Item, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllyEatItemEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetEffectParam<Item>(),
                context.GetTargetOrSourcePokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}