using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnWeather event.
/// Triggered by weather effects.
/// Signature: (Battle battle, Pokemon target, object? source, Condition effect) => void
/// </summary>
public sealed record OnWeatherEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnWeather event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnWeatherEventInfo(
        Action<Battle, Pokemon, object?, Condition> handler,
  int? priority = null,
   bool usesSpeed = true)
    {
  Id = EventId.Weather;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
Priority = priority;
        UsesSpeed = usesSpeed;
ExpectedParameterTypes =
        [
  typeof(Battle),
  typeof(Pokemon),
            typeof(object),
  typeof(Condition),
  ];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnWeatherEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Weather;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnWeatherEventInfo Create(
        Action<Battle, Pokemon, object, Condition> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnWeatherEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon(),
                context.SourcePokemon,
                context.GetSourceEffect<Condition>()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
