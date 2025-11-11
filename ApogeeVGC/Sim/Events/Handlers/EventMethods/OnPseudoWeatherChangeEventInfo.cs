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
    }
}
