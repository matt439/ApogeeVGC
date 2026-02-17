using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnySwitchOut event.
/// Triggered when any Pokemon switches out.
/// Signature: (Battle battle, Pokemon pokemon) => void
/// </summary>
public sealed record OnAnySwitchOutEventInfo : EventHandlerInfo
{
  [Obsolete("Use Create factory method instead.")]
  public OnAnySwitchOutEventInfo(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SwitchOut;
        Prefix = EventPrefix.Any;
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
    public OnAnySwitchOutEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SwitchOut;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnySwitchOutEventInfo Create(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnySwitchOutEventInfo(
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
