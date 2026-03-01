using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyType event (pokemon/ally-specific).
/// Triggered to get ally's type.
/// Signature: Func<Battle, PokemonType[], Pokemon, TypesVoidUnion>
/// </summary>
public sealed record OnAllyTypeEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAllyTypeEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Type;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAllyTypeEventInfo Create(
        Func<Battle, PokemonType[], Pokemon, TypesVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllyTypeEventInfo(
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
