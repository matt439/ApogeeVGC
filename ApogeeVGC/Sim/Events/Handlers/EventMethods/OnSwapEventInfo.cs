using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSwap event.
/// Triggered when two Pokemon swap positions.
/// Signature: (Battle battle, Pokemon target, Pokemon source) => void
/// </summary>
public sealed record OnSwapEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSwap event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnSwapEventInfo(
        Action<Battle, Pokemon, Pokemon> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
 Id = EventId.Swap;
#pragma warning disable CS0618
Handler = handler;
#pragma warning restore CS0618
Priority = priority;
UsesSpeed = usesSpeed;
     ExpectedParameterTypes =
 [
     typeof(Battle),
            typeof(Pokemon),
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
    public OnSwapEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Swap;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSwapEventInfo Create(
        Action<Battle, Pokemon, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSwapEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
