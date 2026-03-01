using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnType event.
/// Triggered to modify a Pokemon's types.
/// Signature: (Battle battle, PokemonType[] types, Pokemon pokemon) => TypesVoidUnion
/// </summary>
public sealed record OnTypeEventInfo : EventHandlerInfo
{
    public OnTypeEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Type;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnTypeEventInfo Create(
        Func<Battle, PokemonType[], Pokemon, TypesVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnTypeEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetRelayVar<TypesRelayVar>().Types.ToArray(),
                context.GetSourceOrTargetPokemon()
                );
                return result switch
                {
                    TypesTypesVoidUnion t => new TypesRelayVar(t.Types),
                    VoidTypesVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
