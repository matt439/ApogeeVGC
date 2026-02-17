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
