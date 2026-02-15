using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnImmunity event.
/// Triggered to check type immunity or weather/condition immunity.
/// Signature: (Battle battle, PokemonTypeConditionIdUnion type, Pokemon pokemon) => BoolVoidUnion
/// </summary>
public sealed record OnImmunityEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnImmunity event handler using the legacy strongly-typed pattern.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnImmunityEventInfo(
        Func<Battle, PokemonTypeConditionIdUnion, Pokemon, BoolVoidUnion> handler,
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
        ExpectedReturnType = typeof(BoolVoidUnion);

        // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false];
        ReturnTypeNullable = false;

        // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, RelayVar (PokemonType or ConditionId), TargetPokemon
    /// </summary>
    public OnImmunityEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Immunity;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnImmunityEventInfo Create(
        Func<Battle, PokemonTypeConditionIdUnion, Pokemon, RelayVar?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnImmunityEventInfo(
            context =>
            {
                PokemonTypeConditionIdUnion typeOrCondition = context.RelayVar switch
                {
                    PokemonTypeRelayVar t => new PokemonTypeConditionIdUnion(t.Type),
                    ConditionIdRelayVar { Id: not null } c => new PokemonTypeConditionIdUnion(c.Id.Value),
                    _ => throw new InvalidOperationException(
                        $"Event {EventId.Immunity}: Cannot resolve PokemonTypeConditionIdUnion from relay var {context.RelayVar?.GetType().Name ?? "null"}")
                };
                return handler(
                    context.Battle,
                    typeOrCondition,
                    context.GetTargetPokemon()
                );
            },
            priority,
            usesSpeed
        );
    }
}