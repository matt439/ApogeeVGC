using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyTryEatItem event (pokemon/ally-specific).
/// Determines if an item can be consumed.
/// Signature: (Battle battle, Item item, Pokemon pokemon) => BoolVoidUnion | bool
/// </summary>
public sealed record OnAllyTryEatItemEventInfo : UnionEventHandlerInfo<OnTryEatItem>
{
    public OnAllyTryEatItemEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TryEatItem;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAllyTryEatItemEventInfo Create(
        Func<Battle, Item, Pokemon, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllyTryEatItemEventInfo(
            context =>
            {
                var result = handler(
                    context.Battle,
                    context.GetEffectParam<Item>(),
                    context.GetTargetOrSourcePokemon()
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
