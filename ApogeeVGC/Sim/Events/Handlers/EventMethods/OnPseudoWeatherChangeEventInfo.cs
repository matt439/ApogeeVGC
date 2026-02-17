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
