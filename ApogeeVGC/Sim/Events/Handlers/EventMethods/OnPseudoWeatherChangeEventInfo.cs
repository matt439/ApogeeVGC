using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnPseudoWeatherChange event.
/// Triggered when pseudo-weather changes.
/// Signature: (Battle battle, Pokemon target, Pokemon source, Condition pseudoWeather) => void
/// </summary>
public sealed record OnPseudoWeatherChangeEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnPseudoWeatherChange event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnPseudoWeatherChangeEventInfo(
      Action<Battle, Pokemon, Pokemon, Condition> handler,
   int? priority = null,
        bool usesSpeed = true)
    {
  Id = EventId.PseudoWeatherChange;
   Handler = handler;
     Priority = priority;
UsesSpeed = usesSpeed;
  ExpectedParameterTypes =
        [
      typeof(Battle),
       typeof(Pokemon),
typeof(Pokemon),
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
    public OnPseudoWeatherChangeEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.PseudoWeatherChange;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnPseudoWeatherChangeEventInfo Create(
        Action<Battle, Pokemon, Pokemon, Condition> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnPseudoWeatherChangeEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetSourceEffect<Condition>()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
