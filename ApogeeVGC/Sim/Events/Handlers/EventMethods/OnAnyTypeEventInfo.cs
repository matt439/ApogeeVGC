using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyType event.
/// Signature: Func<Battle, PokemonType[], Pokemon, TypesVoidUnion>
/// </summary>
public sealed record OnAnyTypeEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAnyTypeEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Type;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnyTypeEventInfo Create(
        Func<Battle, PokemonType[], Pokemon, TypesVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnyTypeEventInfo(
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
