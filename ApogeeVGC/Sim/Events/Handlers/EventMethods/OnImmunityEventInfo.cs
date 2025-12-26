using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnImmunity event.
/// Triggered to check type immunity or weather/condition immunity.
/// Signature: (Battle battle, PokemonTypeConditionIdUnion type, Pokemon pokemon) => void
/// </summary>
public sealed record OnImmunityEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnImmunity event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnImmunityEventInfo(
        Action<Battle, PokemonTypeConditionIdUnion, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Immunity;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
            typeof(Battle),
            typeof(PokemonTypeConditionIdUnion),
            typeof(Pokemon),
        ];
        ExpectedReturnType = typeof(void);

        // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false];
        ReturnTypeNullable = false;

        // Validate configuration
        ValidateConfiguration();
    }
}