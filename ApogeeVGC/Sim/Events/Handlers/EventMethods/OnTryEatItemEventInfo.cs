using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnTryEatItem event.
/// Triggered when a Pokemon tries to eat an item (like a berry).
/// Item parameter is nullable because in RunEvent handler chains, earlier handlers
/// may replace the relay var (from EffectRelayVar to BoolRelayVar), making the
/// original item unresolvable for later handlers.
/// Signature: (Battle battle, Item? item, Pokemon pokemon) => BoolVoidUnion | bool
/// </summary>
public sealed record OnTryEatItemEventInfo : UnionEventHandlerInfo<OnTryEatItem>
{
    public OnTryEatItemEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TryEatItem;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnTryEatItemEventInfo Create(
        Func<Battle, Item?, Pokemon, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnTryEatItemEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.TryGetEffectParam<Item>(),
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
