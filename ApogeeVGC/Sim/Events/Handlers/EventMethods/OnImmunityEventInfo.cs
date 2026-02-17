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
                    context.GetTargetOrSourcePokemon()
                );
            },
            priority,
            usesSpeed
        );
    }
}
