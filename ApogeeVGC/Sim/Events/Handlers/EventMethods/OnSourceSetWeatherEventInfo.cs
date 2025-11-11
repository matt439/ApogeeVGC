using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceSetWeather event.
/// Signature: Func<Battle, Pokemon, Pokemon, Condition, BoolVoidUnion>
/// </summary>
public sealed record OnSourceSetWeatherEventInfo : EventHandlerInfo
{
    public OnSourceSetWeatherEventInfo(
        Func<Battle, Pokemon, Pokemon, Condition, BoolVoidUnion> handler,
    int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.SetWeather;
        Prefix = EventPrefix.Source;
 Handler = handler;
 Priority = priority;
UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(Condition)];
        ExpectedReturnType = typeof(BoolVoidUnion);
    }
}