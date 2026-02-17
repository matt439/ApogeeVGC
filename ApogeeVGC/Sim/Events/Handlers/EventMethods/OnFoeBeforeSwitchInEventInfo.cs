using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeBeforeSwitchIn event.
/// Signature: Action<Battle, Pokemon>
/// </summary>
public sealed record OnFoeBeforeSwitchInEventInfo : EventHandlerInfo
{
 [Obsolete("Use Create factory method instead.")]
 public OnFoeBeforeSwitchInEventInfo(
      Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.BeforeSwitchIn;
   Prefix = EventPrefix.Foe;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
    Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
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
    public OnFoeBeforeSwitchInEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.BeforeSwitchIn;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeBeforeSwitchInEventInfo Create(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeBeforeSwitchInEventInfo(
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
