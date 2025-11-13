using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSwitchIn event.
/// Triggered when a Pokemon switches in.
/// Signature: (Battle battle, Pokemon pokemon) => void
/// </summary>
public sealed record OnSwitchInEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSwitchIn event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSwitchInEventInfo(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SwitchIn;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
            typeof(Battle),
            typeof(Pokemon),
        ];
        ExpectedReturnType = typeof(void);

        // Nullability: Battle (non-null), Pokemon (non-null)
        ParameterNullability = [false, false];
        ReturnTypeNullable = false; // void

        // Validate configuration
        ValidateConfiguration();
    }
}