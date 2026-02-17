using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnEmergencyExit event.
/// Triggered when a Pokemon's HP falls below a certain threshold.
/// Signature: (Battle battle, Pokemon pokemon) => void
/// </summary>
public sealed record OnEmergencyExitEventInfo : EventHandlerInfo
{
    public OnEmergencyExitEventInfo(
        EventHandlerDelegate contextHandler,
        bool usesSpeed = true)
    {
        Id = EventId.EmergencyExit;
        ContextHandler = contextHandler;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnEmergencyExitEventInfo Create(
        Action<Battle, Pokemon> handler,
        bool usesSpeed = true)
    {
        return new OnEmergencyExitEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon()
                );
                return null;
            },
            usesSpeed
        );
    }
}
