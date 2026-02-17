using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeAfterUseItem event.
/// Triggered after a foe Pokemon uses an item.
/// Signature: (Battle battle, Item item, Pokemon pokemon) => void
/// </summary>
public sealed record OnFoeAfterUseItemEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnFoeAfterUseItem event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnFoeAfterUseItemEventInfo(
        Action<Battle, Item, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
      Id = EventId.AfterUseItem;
      Prefix = EventPrefix.Foe;
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
    public OnFoeAfterUseItemEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterUseItem;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeAfterUseItemEventInfo Create(
        Action<Battle, Item, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeAfterUseItemEventInfo(
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
