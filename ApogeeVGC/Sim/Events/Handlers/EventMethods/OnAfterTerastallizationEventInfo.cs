using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAfterTerastallization event.
/// Triggered after a Pokemon Terastallizes.
/// Signature: (Battle battle, Pokemon pokemon) => void
/// </summary>
public sealed record OnAfterTerastallizationEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnAfterTerastallization event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnAfterTerastallizationEventInfo(
  Action<Battle, Pokemon> handler,
   int? priority = null,
        bool usesSpeed = true)
  {
        Id = EventId.AfterTerastallization;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
        Priority = priority;
  UsesSpeed = usesSpeed;
   ExpectedParameterTypes =
        [
            typeof(Battle),
    typeof(Pokemon),
 ];
 ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAfterTerastallizationEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterTerastallization;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAfterTerastallizationEventInfo Create(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAfterTerastallizationEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
