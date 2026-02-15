using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnySwitchIn event.
/// Triggered when any Pokemon switches in.
/// Signature: (Battle battle, Pokemon pokemon) => void
/// </summary>
public sealed record OnAnySwitchInEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnAnySwitchIn event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
 public OnAnySwitchInEventInfo(
Action<Battle, Pokemon> handler,
        int? priority = null,
   bool usesSpeed = true)
  {
Id = EventId.SwitchIn;
        Prefix = EventPrefix.Any;
Handler = handler;
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
    public OnAnySwitchInEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SwitchIn;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnySwitchInEventInfo Create(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnySwitchInEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetTargetPokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
