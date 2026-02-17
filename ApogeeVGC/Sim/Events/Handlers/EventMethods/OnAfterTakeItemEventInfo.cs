using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAfterTakeItem event.
/// Triggered after an item is taken from a Pokemon.
/// Signature: (Battle battle, Item item, Pokemon pokemon) => void
/// </summary>
public sealed record OnAfterTakeItemEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnAfterTakeItem event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnAfterTakeItemEventInfo(
   Action<Battle, Item, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
  Id = EventId.AfterTakeItem;
   #pragma warning disable CS0618
   Handler = handler;
   #pragma warning restore CS0618
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
    public OnAfterTakeItemEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterTakeItem;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAfterTakeItemEventInfo Create(
        Action<Battle, Item, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAfterTakeItemEventInfo(
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
