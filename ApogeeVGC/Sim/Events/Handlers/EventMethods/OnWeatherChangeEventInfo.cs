using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnWeatherChange event.
/// Triggered when weather changes.
/// Signature: (Battle battle, Pokemon target, Pokemon source, IEffect sourceEffect) => void
/// </summary>
public sealed record OnWeatherChangeEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnWeatherChange event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnWeatherChangeEventInfo(
        Action<Battle, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
Id = EventId.WeatherChange;
      Handler = handler;
        Priority = priority;
UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
  [
     typeof(Battle),
 typeof(Pokemon),
 typeof(Pokemon),
   typeof(IEffect),
 ];
  ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
