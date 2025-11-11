using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSetWeather event.
/// Triggered when weather is set.
/// Signature: (Battle battle, Pokemon target, Pokemon source, Condition weather) => BoolVoidUnion
/// </summary>
public sealed record OnSetWeatherEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSetWeather event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
 /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
  public OnSetWeatherEventInfo(
        Func<Battle, Pokemon, Pokemon, Condition, BoolVoidUnion> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SetWeather;
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
      ExpectedReturnType = typeof(BoolVoidUnion);
    }
}
