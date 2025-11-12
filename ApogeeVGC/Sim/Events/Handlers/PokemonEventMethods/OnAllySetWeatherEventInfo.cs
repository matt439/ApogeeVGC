using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllySetWeather event (pokemon/ally-specific).
/// Triggered when setting weather affecting ally.
/// Signature: Func<Battle, Pokemon, Pokemon, Condition, PokemonVoidUnion>
/// </summary>
public sealed record OnAllySetWeatherEventInfo : EventHandlerInfo
{
    public OnAllySetWeatherEventInfo(
    Func<Battle, Pokemon, Pokemon, Condition, PokemonVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SetWeather;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(Condition)];
        ExpectedReturnType = typeof(PokemonVoidUnion);
  }
}