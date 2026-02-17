using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnNegateImmunity event.
/// Negates type immunity for a move.
/// Signature: (Battle battle, Pokemon pokemon, PokemonType? type) => BoolVoidUnion | bool
/// </summary>
public sealed record OnNegateImmunityEventInfo : UnionEventHandlerInfo<OnNegateImmunity>
{
    /// <summary>
    /// Creates a constant-value or delegate-based handler via union type.
    /// </summary>
    public OnNegateImmunityEventInfo(
        OnNegateImmunity unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.NegateImmunity;
        UnionValue = unionValue;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    public OnNegateImmunityEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.NegateImmunity;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnNegateImmunityEventInfo Create(
        Func<Battle, Pokemon, PokemonType, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnNegateImmunityEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon(),
                context.SourceType!.Value
                );
                return result switch
                {
                    BoolBoolVoidUnion b => new BoolRelayVar(b.Value),
                    VoidBoolVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
