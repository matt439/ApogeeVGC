using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnUseItem event.
/// Triggered when a Pokemon uses an item.
/// Signature: (Battle battle, Item item, Pokemon pokemon) => void
/// </summary>
public sealed record OnUseItemEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnUseItem event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnUseItemEventInfo(
        Action<Battle, Item, Pokemon> handler,
   int? priority = null,
  bool usesSpeed = true)
    {
Id = EventId.UseItem;
        Handler = handler;
        Priority = priority;
    UsesSpeed = usesSpeed;
ExpectedParameterTypes =
  [
    typeof(Battle),
      typeof(Item),
     typeof(Pokemon),
        ];
ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
 }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnUseItemEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.UseItem;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnUseItemEventInfo Create(
        Action<Battle, Item, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnUseItemEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetRelayVarEffect<Item>(),
                context.GetTargetOrSourcePokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
